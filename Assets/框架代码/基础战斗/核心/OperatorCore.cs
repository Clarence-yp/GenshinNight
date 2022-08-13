using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Spine;
using UnityEngine;

public class OperatorCore : ElementCore
{
    [Header("干员数据")]
    public operData od_;

    [HideInInspector] public int level;         // 干员等级
    [HideInInspector] public int eliteLevel;    // 干员精英化等级
    [HideInInspector] public int operID;        // 在InitManager的offOperList里的编号
    [HideInInspector] public int skillNum;      // 该干员选择的技能编号
    
    // spine动画相关
    [HideInInspector] public GameObject animObject;
    public Animator anim;
    private SpineAnimController ac_;

    // atkRange相关
    public SearchAndGive atkRange;      // 当前atkRange的脚本
    public AtkRangeShowController atkRangeShowController;
    
    private Action defaultTurn;         // 干员朝默认方向旋转的函数
    
    // 阻挡相关
    private int block;                  // 当前剩余可用阻挡数
    private Dictionary<EnemyCore, bool> alreadyBlockSet = new Dictionary<EnemyCore, bool>();

    private void Awake()
    {
        animObject = transform.Find("anim").gameObject;
        anim = animObject.GetComponent<Animator>();
        ac_ = new SpineAnimController(anim, 0.3f);
        atkRangeShowController = new AtkRangeShowController(this);
        
        
        InitCalculation();      // 初始化battleCalculation
        ChangeAtkRange();       // 生成atkRange
    }

    protected override void Start_ElementCore_Down()
    {
        OperInit();
        
        Start_OperatorCore_Down();
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

        // 初始化当前阻挡数
        block = battleCalculation.maxBlock;
    }

    private void InitCalculation()
    {
        battleCalculation.atk_.ChangeBaseValue(od_.atk);
        battleCalculation.def_.ChangeBaseValue(od_.def);
        battleCalculation.magicDef_.ChangeBaseValue(od_.magicDef);
        battleCalculation.life_.ChangeBaseValue(od_.life);
        battleCalculation.maxBlock = od_.maxBlock;
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
        BattleCore battleCore = other.GetComponent<BattleCore>();
        blockList.Add(battleCore);
        battleCore.battleCore_DieAction += DelBattleCore_OperBlock;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("enemy")) return;
        BattleCore battleCore = other.GetComponent<BattleCore>();
        battleCore.battleCore_DieAction -= DelBattleCore_OperBlock;
        DelBattleCore_OperBlock(battleCore);
    }

    // 敌人死亡时，处理block相关的回调函数
    private void DelBattleCore_OperBlock(BattleCore bc_)
    {
        blockList.Remove(bc_);
        EnemyCore ec_ = (EnemyCore)bc_;
        if (alreadyBlockSet.ContainsKey(ec_) && alreadyBlockSet[ec_])
        {
            block += ec_.battleCalculation.maxBlock;
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
            
            if (ec_.battleCalculation.maxBlock <= block)
            {
                ec_.BeBlocked();
                block -= ec_.battleCalculation.maxBlock;
                alreadyBlockSet[ec_] = true;
            }
        }
    }

    
    /// <summary>
    /// 摧毁旧的atkRange，根据目前的精英化等级生成一个新的atkRange
    /// </summary>
    public void ChangeAtkRange()
    {
        // 记录当前的rotation，更换后保持不变
        Quaternion rol;
        rol = Quaternion.Euler(0, 0, 0);
        if (atkRange != null) rol = atkRange.transform.rotation;
        
        foreach (var i in operatorList)
        {
            atkRange.pretendExit(i.gameObject);
        }
        foreach (var i in enemyList)
        {
            atkRange.pretendExit(i.gameObject);
        }

        if (atkRange != null) Destroy(atkRange.gameObject);
        
        GameObject newRange = Instantiate(od_.atkRange[eliteLevel]
            , transform, true);
        newRange.transform.localPosition=Vector3.zero;
        newRange.transform.rotation = rol;
        atkRange = newRange.GetComponent<SearchAndGive>();
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
        battleCalculation.Atk();
        battleCalculation.Battle(this, target, 1, DamageMode.Physical);
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

public class AtkRangeShowController
{
    private List<GameObject> showingRangeImage = new List<GameObject>();
    private OperatorCore oc_;

    public AtkRangeShowController(OperatorCore ooc)
    {
        oc_ = ooc;
    }
    
    
    /// <summary>
    /// 在世界方块上展示攻击范围
    /// </summary>
    public void ShowAtkRange()
    {
        float rol_y = oc_.atkRange.transform.rotation.eulerAngles.y;
        foreach (var detaPos in oc_.atkRange.RangePos)
        {
            Vector3 pos = new Vector3();
            if (rol_y == 0)
            {
                pos.x = detaPos.x;
                pos.z = detaPos.y;
            }
            if (rol_y == 270)
            {
                pos.x = -detaPos.y;
                pos.z = detaPos.x;
            }
            if (rol_y == 180)
            {
                pos.x = -detaPos.x;
                pos.z = detaPos.y;
            }
            if (rol_y == 90)
            {
                pos.x = -detaPos.y;
                pos.z = -detaPos.x;
            }
            pos += oc_.anim.transform.position;
            pos.z -= BaseFunc.operAnimFix_z;

            if (InitManager.GetMap(pos) == null) continue;
            if (Interpreter.isHigh(InitManager.GetMap(pos).type))
                pos.y = BaseFunc.highOper_y + 0.01f;
            else pos.y = 0.01f;
            
            GameObject showing = PoolManager.GetObj(SpriteElement.atkRangeImage);
            showing.transform.position = pos;
            showingRangeImage.Add(showing);
        }
    }

    /// <summary>
    /// 销毁展示的攻击范围
    /// </summary>
    public void HideAtkRange()
    {
        foreach (var i in showingRangeImage)
        {
            PoolManager.RecycleObj(i);
        }
        showingRangeImage.Clear();
    }
}