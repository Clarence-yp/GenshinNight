using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCore : MonoBehaviour
{
    [HideInInspector] public List<ElementCore> operatorList = new List<ElementCore>();
    [HideInInspector] public List<ElementCore> enemyList = new List<ElementCore>();
    [HideInInspector] public AimingMode aimingMode;
    [HideInInspector] public float tarPriority = 0;         // 在别的BattleCore队列中排序的参照，由外部维护
    public bool dieNow = false;     //调试变量，立即杀死自身
    
    // BattleCore瞄准的目标
    public ElementCore target { get; private set; } = null;
    public bool tarIsNull { get; private set; } = true;

    // 当死亡时给外界广播回调用的函数
    public Action<ElementCore> public_DieAction;
    // 当死亡时给自身子类调用的函数，最后调用
    protected Action dieAction;
    
    // 阻挡的BattleCore列表
    [HideInInspector] public List<ElementCore> blockList = new List<ElementCore>();

    
    private void Start()
    {
        
        Start_BattleCore_Down();
    }

    protected virtual void Start_BattleCore_Down() {}

    private void Update()
    {
        
        ChooseTarget();
        
        
        Update_BattleCore_Down();
    }

    protected virtual void Update_BattleCore_Down() {}

    protected virtual int operCmp(BattleCore a, BattleCore b)
    {
        // 给攻击范围内的干员排序，默认以priority（放置顺序）从大到小排
        if (a.tarPriority > b.tarPriority) return -1;
        if (a.tarPriority < b.tarPriority) return 1;
        return 0;
    }

    protected virtual int enemyCmp(BattleCore a, BattleCore b)
    {
        // 给攻击范围内的敌人排序，默认以priority（离终点距离）从小到大排
        if (a.tarPriority > b.tarPriority) return 1;
        if (a.tarPriority < b.tarPriority) return -1;
        return 0;
    }
    
    private void ChooseTarget()
    {
        operatorList.Sort(operCmp);
        enemyList.Sort(enemyCmp);

        if (blockList.Count > 0)
        {
            target = blockList[0];
            tarIsNull = false;
            return;
        }
        
        if (aimingMode == AimingMode.enemyFirst)
        {
            if (enemyList.Count == 0)
            {
                tarIsNull = true;
            }
            else
            {
                target = enemyList[0];
                tarIsNull = false;
            }
        }
        else if (aimingMode == AimingMode.operatorFirst)
        {
            if (operatorList.Count == 0)
            {
                tarIsNull = true;
            }
            else
            {
                target = operatorList[0];
                tarIsNull = false;
            }
        }
    }
    

    
}

public enum AimingMode
{
    operatorFirst,
    enemyFirst
}

public enum DamageMode
{
    Physical,
    Magic
}

public class BattleCalculation
{
    // 战斗计算器，同样也是基本属性储存区

    // 基础数据
    public ValueBuffer atk_ = new ValueBuffer(0);
    public ValueBuffer def_ = new ValueBuffer(0);
    public ValueBuffer magicDef_ = new ValueBuffer(0);
    public LifeController life_ = new LifeController();

    // 进阶数据
    public float minAtkInterval = 2.5f;     // 最小攻击间隔
    public int maxBlock = 0;                // 最大阻挡数（敌人为消耗阻挡数）
    
    // 本地数据
    private float atkInterval = 0;      // 到下一次攻击还需要的时间

    // 造成伤害增加buff，val值表示百分比增加幅度（不可小于-1）
    private ValueBuffer causeDamInc_ = new ValueBuffer(0);
    // 造成伤害改变委托列表，由其他类注册，前委托的输出会作为后委托的输入
    private List<Func<float, float>> causeDamFuncList = new List<Func<float, float>>();
    // 受到伤害增加buff，val值表示百分比增加幅度（不可小于-1）
    private ValueBuffer getDamInc_ = new ValueBuffer(0);
    // 受到伤害改变委托列表，由其他类注册，前委托的输出会作为后委托的输入
    private List<Func<float, float>> getDamFuncList = new List<Func<float, float>>();


    public virtual void Update()
    {
        // 该函数需在其它函数的Update里调用才会生效
        atkInterval = atkInterval - Time.deltaTime <= 0 ? 0 : atkInterval - Time.deltaTime;
    }
    
    /// <summary>  
    /// 将baseDamage过一遍BattleCore里的buff
    /// </summary>
    protected float Bat_CauseDamage(float dam)
    {
        dam *= causeDamInc_.val < -1 ? 0 : 1 + causeDamInc_.val;
        foreach (var damFunc in causeDamFuncList)
            dam = damFunc(dam);
        return dam;
    }
    
    /// <summary>  
    /// 该BattleCore受到一次伤害，计算并改变life_的值
    /// </summary>
    protected void Bat_GetDamage(float baseDamage, DamageMode mode)
    {
        float dam = baseDamage;
        dam *= getDamInc_.val < -1 ? 0 : 1 + getDamInc_.val;
        foreach (var damFunc in getDamFuncList)
            dam = damFunc(dam);

        if (mode == DamageMode.Physical)
            dam = dam - def_.val < 0 ? 0 : dam - def_.val;
        else if (mode == DamageMode.Magic)
            dam = dam * (1 - (magicDef_.val / 100));
        
        life_.GetDamage(dam);
    }
    

    /// <summary>  
    /// 表示该BattleCore进行了一次普攻，开始冷却
    /// </summary>
    public void NorAtkStartCool()
    {
        atkInterval = minAtkInterval;
    }
    
    public bool CanAtk()
    {
        return atkInterval <= 0;
    }
    
}

public enum ValueBuffMode
{
    Fixed,
    Percentage
}

public class ValueBuffSlot
{
    public ValueBuffMode mode;
    public float v;
}

public class ValueBuffer
{
    // 给buff使用的数值缓冲区

    public float val { get; private set; }
    public float baseVal { get; private set; }
    private List<ValueBuffSlot> valueBuffList = new List<ValueBuffSlot>();

    public ValueBuffer()
    {
        val = baseVal = 0;
    }
    public ValueBuffer(float v)
    {
        val = baseVal = v;
    }

    private void RefreshValue()
    {
        val = baseVal;
        foreach (var buffSlot in valueBuffList)
        {
            if (buffSlot.mode == ValueBuffMode.Fixed)
                val += buffSlot.v;
            else if(buffSlot.mode == ValueBuffMode.Percentage)
                val += buffSlot.v * baseVal;
        }
    }
    
    /// <summary>  
    /// 更改基础数值
    /// </summary>
    public void ChangeBaseValue(float newBaseVal)
    {
        baseVal = newBaseVal;
        RefreshValue();
    }
    
    /// <summary>  
    /// 增加基础数值
    /// </summary>
    public void AddBaseValue(float addBaseVal)
    {
        ChangeBaseValue(baseVal + addBaseVal);
    }
    
    
    /// <summary>  
    /// 加入新的数值buff
    /// </summary>
    public void AddValueBuff(ValueBuffSlot buffSlot)
    {
        valueBuffList.Add(buffSlot);
        RefreshValue();
    }
    
    /// <summary>  
    /// 移除已有的数值buff
    /// </summary>
    public void DelValueBuff(ValueBuffSlot buffSlot)
    {
        valueBuffList.Remove(buffSlot);
        RefreshValue();
    }
}

public class LifeController : ValueBuffer
{
    // lifeController父类的val为最大生命值，子类的life为当前生命值
    public float life { get; private set; }

    public void InitBaseLife(float v)
    {
        ChangeBaseValue(v);
        life = v;
    }

    public void GetDamage(float damage)
    {
        life = life - damage < 0 ? 0 : life - damage;
    }

    public void GetHeal(float heal)
    {
        life = life + heal > val ? val : life + heal;
    }
}

public class SPController
{
    public float sp;
    public float maxSp;

    public bool during;         // 技能是否开启
    public float remainingTime; // 技能剩余持续时间
    public float maxTime;       // 技能最大持续时间

    
}


