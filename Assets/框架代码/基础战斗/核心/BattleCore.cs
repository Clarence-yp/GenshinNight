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
    
    [HideInInspector] public float norAtkInterval = 0;         // 到下一次攻击还需要的时间
    // [HideInInspector] public bool nxtAtkImmediately = false;      // 下一次退出攻击状态时，立刻清空冷却并进入攻击状态
    [HideInInspector] public bool fighting;
    
    // 进阶数据
    [HideInInspector] public AtkSpeedController atkSpeedController;
    [HideInInspector] public ValueBuffer maxBlock = new ValueBuffer(0); // 最大阻挡数（敌人为消耗阻挡数）
    [HideInInspector] public bool dying;
    [HideInInspector] public int dizziness;         // 眩晕计数器，>0时表示处于眩晕状态
    
    // 显示在场上的，动画/物体位置
    [HideInInspector] public Transform animTransform;

    // 血条canvas
    [HideInInspector] public Canvas frontCanvas;
    
    // BattleCore瞄准的目标
    public BattleCore target { get; private set; } = null;
    public bool tarIsNull { get; private set; } = true;

    // 当死亡时给外界广播回调用的函数
    public Action<BattleCore> DieAction;

    // 阻挡的BattleCore列表
    [HideInInspector] public List<BattleCore> blockList = new List<BattleCore>();


    protected override void Start_Core()
    {
        base.Start_Core();
    }
    
    protected override void Update_Core()
    {
        base.Update_Core();
        ChooseTarget();
        CheckDie();
        atkSpeedController.Update();
        if (!frozen)         // 如果处于冻结状态，攻击计时器不再运作
            norAtkInterval = norAtkInterval - Time.deltaTime <= 0 ? 0 : norAtkInterval - Time.deltaTime;

        if (dieNow) // 测试用，后期删掉
            GetDamage(1e9f, DamageMode.Magic);
    }
    
    
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
    
    public virtual void GetDizzy()
    {// 眩晕
        dizziness++;
    }

    public virtual void RevokeDizzy()
    {// 眩晕结束
        dizziness--;
        norAtkInterval = 0;
    }

    /// <summary>  
    /// 自身对tarBC造成一次伤害，结束后更新彼此数值
    /// </summary>
    public void Battle(BattleCore tarBC, float damage, DamageMode mode, // 造成伤害的基础数值，以及本次伤害类型
        ElementSlot elementSlot, ElementTimer timer,    // 元素攻击，以及使用的元素计时器
        bool haveText = false, bool isBig = false)      // 显示攻击数字                    
    {
        bool canAttachElement = CauseDamageElement(
            tarBC, ref damage, elementSlot, timer);
        tarBC.GetDamage(this, damage, mode, elementSlot,
            canAttachElement, haveText, isBig);
    }

    public void Battle(BattleCore tarBC, float damage, DamageMode mode, ElementSlot elementSlot)
    {
        Battle(tarBC, damage, mode, elementSlot, defaultElementTimer);
    }

    public void Battle(BattleCore tarBC, float damage, DamageMode mode = DamageMode.Physical)
    {
        ElementSlot phy = new ElementSlot();
        Battle(tarBC, damage, mode, phy, defaultElementTimer);
    }
    
    
    /// <summary>  
    /// 表示该BattleCore进行了一次普攻，开始冷却
    /// </summary>
    public void NorAtkStartCool()
    {
        norAtkInterval = atkSpeedController.minAtkInterval;
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
    public Animator anim;
    public SpineAnimController ac_;
    public BattleCore bc_;
    public ValueBuffer atkSpeed;        // 攻击速度加成，100表示最小攻击间隔减小一半
    public float minAtkInterval;        // 最小攻击间隔
    public float baseInterval;          // 基础攻击间隔
    
    private float fightAnimTime;

    public AtkSpeedController(BattleCore bc, SpineAnimController controller, float aspeed, float interval)
    {
        bc_ = bc;
        ac_ = controller;
        anim = ac_.anim;
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
        ac_.atkSpeed = nspeed;
        ac_.ChangeAnimSpeed();
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
        if (atkSpeed.val == 0)
        {
            ac_.atkSpeed = 1;
            ac_.ChangeAnimSpeed();
        }
    }
}

public class SkillAtkSpeedBuff : SkillBuffSlot
{
    private AtkSpeedController atkSpeedController;
    private float atkSpeedInc;
    private ValueBuffInner buffInner;
    private float baseInterval;
    
    private float p_baseInterval;

    public SkillAtkSpeedBuff(AtkSpeedController controller, float atkSpeedInc_, BattleCore bc,
        float interval = -1) : base(bc)
    {
        atkSpeedController = controller;
        atkSpeedInc = atkSpeedInc_;
        buffInner = new ValueBuffInner(ValueBuffMode.Fixed, atkSpeedInc);
        baseInterval = interval;

        p_baseInterval = atkSpeedController.baseInterval;
    }

    public override void BuffStart()
    {
        atkSpeedController.atkSpeed.AddValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        ReSetIntervalTime();
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(baseInterval);
    }

    public override void BuffEnd()
    {
        atkSpeedController.atkSpeed.DelValueBuff(buffInner);
        atkSpeedController.RefreshInterval();
        ReSetIntervalTime();
        if (baseInterval > 0) atkSpeedController.ChangeBaseInterval(p_baseInterval);
    }
    
    public override void BuffUpdate() { }

    private void ReSetIntervalTime()
    {
        Animator anim = atkSpeedController.anim;
        BattleCore bc_ = atkSpeedController.bc_;
        
        var staInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (!staInfo.IsName("Fight"))
        {
            bc_.norAtkInterval = 0;
            return;
        }
        
        float fightAnimTime = staInfo.length;
        if (fightAnimTime - atkSpeedController.minAtkInterval < 0.008f) return;
        float nspeed = (fightAnimTime / atkSpeedController.minAtkInterval) + 0.005f;
        float conTime = (staInfo.normalizedTime - (int) staInfo.normalizedTime) * fightAnimTime / nspeed;
        bc_.norAtkInterval = atkSpeedController.minAtkInterval - conTime;
    }
    
}

public class DurationDizzyBuff : DurationBuffSlot
{
    private BattleCore dizzyBC_;

    public DurationDizzyBuff(BattleCore bc_, float durTime) : base(durTime)
    {
        dizzyBC_ = bc_;
    }

    public override void BuffStart()
    {
        dizzyBC_.GetDizzy();
    }

    public override void BuffEnd()
    {
        dizzyBC_.RevokeDizzy();
    }
}