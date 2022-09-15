using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCore : ElementCore
{
    [HideInInspector] public List<BattleCore> operatorList = new List<BattleCore>();
    [HideInInspector] public List<BattleCore> enemyList = new List<BattleCore>();
    [HideInInspector] public AimingMode aimingMode;
    [HideInInspector] public float tarPriority = 0;         // 在别的BattleCore队列中排序的参照，由外部维护
    public bool dieNow = false;     //调试变量，立即杀死自身
    
    protected float norAtkInterval = 0;         // 到下一次攻击还需要的时间
    public bool nxtAtkImmediately = false;      // 下一次退出攻击状态时，立刻清空冷却并进入攻击状态
    public bool fighting;
    
    // 进阶数据
    [HideInInspector] public AtkSpeedController atkSpeedController;
    [HideInInspector] public ValueBuffer maxBlock = new ValueBuffer(0); // 最大阻挡数（敌人为消耗阻挡数）
    [HideInInspector] public bool dying;
    
    
    // BattleCore瞄准的目标
    public BattleCore target { get; private set; } = null;
    public bool tarIsNull { get; private set; } = true;

    // 当死亡时给外界广播回调用的函数
    public Action<BattleCore> DieAction;

    // 阻挡的BattleCore列表
    [HideInInspector] public List<BattleCore> blockList = new List<BattleCore>();


    protected override void Start_ElementCore_Down()
    {
        Start_BattleCore_Down();
    }

    protected virtual void Start_BattleCore_Down() {}

    protected override void Update_ElementCore_Down()
    {
        ChooseTarget();
        CheckDie();
        atkSpeedController.Update();
        norAtkInterval = norAtkInterval - Time.deltaTime <= 0 ? 0 : norAtkInterval - Time.deltaTime;

        if (dieNow) // 测试用，后期删掉
            GetDamage(1e9f, DamageMode.Magic);
        
        Update_BattleCore_Down();
    }

    protected virtual void Update_BattleCore_Down() {}
    
    private void CheckDie()
    {
        if (life_.life <= 0 && !dying)
        {
            dying = true;
            if (DieAction != null)
            {
                DieAction(this);
                DieAction = null;
            }
            DieBegin();
        }
    }

    protected virtual void DieBegin() {}

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
    
    /// <summary>  
    /// attacker对defender造成一次伤害，结束后更新彼此数值
    /// </summary>
    public void Battle(BattleCore attacker, BattleCore defender,
        float multi, DamageMode mode, ElementSlot elementSlot)
    {
        float dam = attacker.CauseDamage(multi, elementSlot);
        defender.GetDamage(dam, mode, elementSlot);
    }
    
    public void Battle(BattleCore attacker, BattleCore defender, float multi, DamageMode mode)
    {
        Battle(attacker, defender, multi, mode, new ElementSlot());
    }
    
    
    /// <summary>  
    /// 表示该BattleCore进行了一次普攻，开始冷却
    /// </summary>
    public void NorAtkStartCool()
    {
        norAtkInterval = atkSpeedController.minAtkInterval;
    }
    
    public void ClearAtkInterval()
    {
        norAtkInterval = 0;
    }
    
    /// <summary>  
    /// 表示该Core是否可以进行普攻
    /// </summary>
    protected bool CanAtk()
    {
        bool k = true;
        k = k & (norAtkInterval <= 0);
        return k;
    }
    
}

public enum AimingMode : byte
{
    [EnumLabel("瞄准干员")]
    operatorFirst,
    [EnumLabel("瞄准敌人")]
    enemyFirst
}

public class AtkSpeedController
{
    private Animator anim;
    public BattleCore bc_;
    public ValueBuffer atkSpeed;        // 攻击速度加成，100表示最小攻击间隔减小一半
    public float minAtkInterval;        // 最小攻击间隔
    public float baseInterval;          // 基础攻击间隔
    
    private float fightAnimTime;

    public AtkSpeedController(BattleCore bc, Animator animator, float aspeed, float interval)
    {
        bc_ = bc;
        anim = animator;
        atkSpeed = new ValueBuffer(aspeed);
        ChangeBaseInterval(interval);
    }

    public void Update()
    {
        if (atkSpeed.val == 0 || bc_.fighting == false) return;
        var staInfo = anim.GetCurrentAnimatorStateInfo(0);
        fightAnimTime = staInfo.length;
        if (!staInfo.IsName("Fight")) return;
        if (fightAnimTime - minAtkInterval < 0.008f) return;

        float nspeed = (fightAnimTime / minAtkInterval) + 0.005f;
        anim.speed = nspeed;
    }
    
    public void ChangeBaseInterval(float interval)
    {
        baseInterval = interval;
        RefreshInterval();
    }

    public void RefreshInterval()
    {
        float tmp = 1 / (1 + atkSpeed.val / 100);
        minAtkInterval = baseInterval * tmp;
        if (atkSpeed.val == 0) anim.speed = 1;
    }
}

public class DurationAtkSpeedBuff : DurationBuffSlot
{
    private AtkSpeedController atkSpeedController;
    private float atkSpeed;
    private ValueBuffInner buffInner;
    private float baseInterval;
    
    private float p_baseInterval;

    public DurationAtkSpeedBuff(AtkSpeedController controller, float speed, float durTime,
        float interval = -1)
    {
        atkSpeedController = controller;
        atkSpeed = speed;
        buffInner = new ValueBuffInner(ValueBuffMode.Fixed, atkSpeed);
        during = durTime;
        baseInterval = interval;

        p_baseInterval = atkSpeedController.baseInterval;
    }

    public override void BuffStart()
    {
        atkSpeedController.atkSpeed.AddValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        atkSpeedController.bc_.nxtAtkImmediately = true;
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(baseInterval);
    }

    public override void BuffEnd()
    {
        atkSpeedController.atkSpeed.DelValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        // atkSpeedController.bc_.nxtAtkImmediately = true;
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(p_baseInterval);
    }
}