using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Spine;
using UnityEditor;
using UnityEngine;

public class OperatorCore : ElementCore
{
    [Header("干员数据")]
    public operData od_;

    [HideInInspector] public int level;         // 干员等级
    [HideInInspector] public int eliteLevel;    // 干员精英化等级
    [HideInInspector] public int operID;        // 在InitManager的offOperList里的编号
    [HideInInspector] public int skillNum;      // 该干员选择的技能编号

    public bool prePutOn;           // 一开始就在场上的
    
    // spine动画相关
    [HideInInspector] public GameObject animObject;
    public Animator anim;
    private SpineAnimController ac_;

    // atkRange相关
    public SearchAndGive atkRange;      // 当前atkRange的脚本

    private Action defaultTurn;         // 干员朝默认方向旋转的函数
    
    // 阻挡相关
    private int block;                  // 当前剩余可用阻挡数
    private Dictionary<EnemyCore, bool> alreadyBlockSet = new Dictionary<EnemyCore, bool>();

    private void Awake()
    {
        animObject = transform.Find("anim").gameObject;
        anim = animObject.GetComponent<Animator>();
        ac_ = new SpineAnimController(anim, 0.3f);
        
        // 这里是注册所有干员atkRange信息
        foreach (var atkRange in od_.atkRange)
        {
            GameObject tmp = Instantiate(atkRange);
            Destroy(tmp);
        }

        InitCalculation();      // 初始化battleCalculation
        ChangeAtkRange();       // 生成atkRange
    }

    protected override void Start_ElementCore_Down()
    {
        OperInit();

        Start_OperatorCore_Down();

        if (prePutOn) return;
        gameObject.SetActive(false);
    }

    protected virtual void Start_OperatorCore_Down() {}
    
    protected override void Update_ElementCore_Down()
    {
        Fight();
        CheckBlock();
        
        ac_.Update();
        Update_OperatorCore_Down();
    }
    
    protected virtual void Update_OperatorCore_Down() {}

    public void OperInit()
    {
        // 在每次登场时的初始化函数，用于初始化本OperatorCore
        
        // 根据当前朝向设定默认朝向
        if (anim.transform.localScale.x > 0) defaultTurn = ac_.TurnRight;
        else defaultTurn = ac_.TurnLeft;
        
        // 让Anim开始播放动画
        anim.SetBool("start", true);

    }

    private void InitCalculation()
    {
        fightCalculation.atk_.ChangeBaseValue(od_.atk);
        fightCalculation.def_.ChangeBaseValue(od_.def);
        fightCalculation.magicDef_.ChangeBaseValue(od_.magicDef);
        fightCalculation.life_.InitBaseLife(od_.life);
        fightCalculation.maxBlock = od_.maxBlock;

        fightCalculation.mastery.ChangeBaseValue(od_.elementalMastery);
        fightCalculation.elementDamage.ChangeBaseValue(od_.elementalDamage);
        fightCalculation.elementResistance.ChangeBaseValue(od_.elementalResistance);
        fightCalculation.spRecharge.ChangeBaseValue(od_.spRecharge);
    }

    
    private void Fight()
    {
        if (!tarIsNull)
        {
            if (CanAtk())
            {
                anim.SetBool("fight", true);
                // 根据目标位置转变干员朝向
                Vector2 detaPos = BaseFunc.xz(transform.position) - BaseFunc.xz(target.transform.position);
                if (detaPos.x < 0) ac_.TurnRight();
                else ac_.TurnLeft();
            }
            else
            {
                anim.SetBool("fight", false);
            }
        }
        else
        {
            anim.SetBool("fight", false);
            defaultTurn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        ElementCore elementCore = other.GetComponent<ElementCore>();
        blockList.Add(elementCore);
        elementCore.public_DieAction += DelBattleCore_OperBlock;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        ElementCore elementCore = other.GetComponent<ElementCore>();
        elementCore.public_DieAction -= DelBattleCore_OperBlock;
        DelBattleCore_OperBlock(elementCore);
    }

    // 敌人死亡时，处理block相关的回调函数
    private void DelBattleCore_OperBlock(ElementCore bc_)
    {
        blockList.Remove(bc_);
        EnemyCore ec_ = (EnemyCore)bc_;
        if (alreadyBlockSet.ContainsKey(ec_) && alreadyBlockSet[ec_])
        {
            block += ec_.fightCalculation.maxBlock;
            alreadyBlockSet[ec_] = false;
            ec_.UnBlocked();
        }
    }

    void CheckBlock()
    {
        foreach (var i in blockList)
        {
            EnemyCore ec_ = (EnemyCore)i;
            if (alreadyBlockSet.ContainsKey(ec_) && alreadyBlockSet[ec_]) continue;
            
            if (ec_.fightCalculation.maxBlock <= block)
            {
                ec_.BeBlocked();
                block -= ec_.fightCalculation.maxBlock;
                alreadyBlockSet[ec_] = true;
            }
        }
    }

    
    /// <summary>
    /// 摧毁旧的atkRange，生成一个新的atkRange，同时更新block的值
    /// </summary>
    public void ChangeAtkRange(GameObject NewRange)
    {
        // 记录当前的rotation，更换后保持不变
        Quaternion rol;
        rol = Quaternion.Euler(0, 0, 0);
        if (atkRange != null) rol = atkRange.transform.rotation;

        while (operatorList.Count > 0)
        {
            atkRange.pretendExit(operatorList[0].gameObject);
        }
        while (enemyList.Count > 0)
        {
            atkRange.pretendExit(enemyList[0].gameObject);
        }

        if (atkRange != null) Destroy(atkRange.gameObject);

        GameObject newRange = Instantiate(NewRange, transform, true);
        newRange.transform.localPosition=Vector3.zero;
        newRange.transform.rotation = rol;
        atkRange = newRange.GetComponent<SearchAndGive>();
        
        // 初始化当前阻挡数
        block = fightCalculation.maxBlock;
    }
    
    /// <summary>
    /// 生成当前精英化等级的atkRange
    /// </summary>
    public void ChangeAtkRange()
    {
        ChangeAtkRange(od_.atkRange[eliteLevel]);
    }
    
    /// <summary>
    /// 点击干员时调用的函数
    /// </summary>
    public void OnClick()
    {
        InitManager.TimeSlowPick(transform);
        OperUIManager.OpenOperUI(UIstate.UP, this);
    }
    
    

    /// <summary>
    /// 在登场动画关键帧调用
    /// </summary>
    public virtual void OnStart()
    {
        
    }

    /// <summary>
    /// 普通攻击，在攻击动画关键帧调用
    /// </summary>
    public virtual void OnAttack()
    {
        fightCalculation.NorAtkStartCool();
        fightCalculation.Fight(this, target, 1, DamageMode.Physical);
    }
    
    public virtual void Skill1() { }
    
    public virtual void Skill2() { }
    
    public virtual void Skill3() { }

}

public class SpineAnimController
{
    private const float turnSpeed = 10;
    private const float colorSpeed = 10;
    
    private Animator anim;
    private MeshRenderer meshRenderer;
    
    public bool dirRight { get; private set; }   //模型是否朝右
    private Vector3 tarScale;
    private Vector3 leftScale;
    private Vector3 rightScale;
    
    private Action colorAction;
    private bool colorChanging = false;
    private Color tarColor = new Color32(255, 255, 255, 255);
    private static readonly Color defaultColor = new Color32(255, 255, 255, 255);

    public SpineAnimController(Animator animator, float scale)
    {
        anim = animator;
        meshRenderer = anim.GetComponent<MeshRenderer>();
        leftScale = new Vector3(-scale, scale, scale);
        rightScale = new Vector3(scale, scale, scale);
        tarScale = rightScale;
    }

    public void Update()
    {
        // 该函数需在其它函数的Update里调用才会生效
        
        // 旋转
        anim.transform.localScale =
            Vector3.Lerp(anim.transform.localScale, tarScale, turnSpeed * Time.deltaTime);
        
        ColorAnim();
    }
    
    

    private void ColorAnim()
    {
        if (colorChanging)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            mpb.SetColor("_Color", Color.Lerp(mpb.GetColor("_Color"), tarColor,
                colorSpeed * Time.deltaTime));
            meshRenderer.SetPropertyBlock (mpb);

            Color color = mpb.GetColor("_Color");
            float det = color.r - tarColor.r + color.b - tarColor.b + color.g - tarColor.g + color.a - tarColor.a;
            if (Math.Abs(det) < 0.01)
            {
                colorChanging = false;
                colorAction?.Invoke();
            }
        }
    }

    /// <summary>
    /// 改变动画播放速度为speed
    /// </summary>
    public void ChangeAnimSpeed(float speed)
    {
        anim.speed = speed;
    }
    
    /// <summary>
    /// 调用一次，动画将旋转面向左边
    /// </summary>
    public void TurnLeft()
    {
        dirRight = false;
        tarScale = leftScale;
    }

    /// <summary>
    /// 调用一次，动画将旋转面向右边
    /// </summary>
    public void TurnRight()
    {
        dirRight = true;
        tarScale = rightScale;
    }
    
    /// <summary>
    /// 调用一次，人物将进行一次闪烁，目标颜色为color
    /// </summary>
    public void TwinkColor(Color color)
    {
        tarColor = color;
        colorAction += colorAction_Recover;
        colorChanging = true;
    }

    private void colorAction_Recover()
    {
        tarColor = defaultColor;
        colorAction -= colorAction_Recover;
        colorChanging = true;
    }
    
    /// <summary>
    /// 调用一次，人物颜色将渐变为目标颜色为color
    /// </summary>
    public void ChangeColor(Color color)
    {
        tarColor = color;
        colorChanging = true;
    }
    
    /// <summary>
    /// 调用一次，人物颜色将渐变为默认颜色
    /// </summary>
    public void ChangeDefaultColor()
    {
        tarColor = defaultColor;
        colorChanging = true;
    }
    
}


