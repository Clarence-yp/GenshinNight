using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class OperUIManager : MonoBehaviour
{
    public static OperUIManager instance;

    public static OperatorCore showingOper;     // 正在展示的干员
    public static bool showing;                 // 正在展示
    public static UIstate sta;                  // 当前展示的模式

    private static gradualChange gc_;           // 整体UI的渐变控制


    // Controllers
    public static RightUIController rightUIController;
    public static LevelUIController levelUIController;
    public static SkillUIController skillUIController;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        
        gc_ = instance.transform.Find("InfoCanvas").GetComponent<gradualChange>();
    }

    private void Start()
    {
        rightUIController = new RightUIController();
        levelUIController = new LevelUIController();
        skillUIController = new SkillUIController();
    }

    private void Update()
    {
        if (!showing) return;
        rightUIController.Update();
        levelUIController.Update();
    }

    /// <summary>
    /// 每关开始Start时调用
    /// </summary>
    public static void Init()
    {
        
    }

    /// <summary>
    /// 刷新整个OperUI
    /// </summary>
    public static void Refresh()
    {
        rightUIController.Refresh();
        levelUIController.Refresh();
        skillUIController.Refresh();
    }
    
    private static void StartAllController()
    {
        // 使用sta初始化所有controller
        rightUIController.SetBySta();
        levelUIController.SetBySta();
        skillUIController.SetBySta();
    }

    private static void CloseAllController()
    {
        rightUIController.Close();
    }

    public static void OpenOperUI(UIstate sta_, OperatorCore showingOper_)
    {
        sta = sta_;
        showingOper = showingOper_;
        showing = true;
        StartAllController();           // 初始化所有Controller
        gc_.gameObject.SetActive(true);
        gc_.Show();
    }

    public static void CloseOperUI()
    {
        CloseAllController();
        showingOper = null;
        showing = false;
        gc_.ImmediateHide();
        InitManager.TimeRecover();
        gc_.gameObject.SetActive(false);
    }
    
    
}

public enum UIstate
{
    Dragging,
    Down,
    UP
}

public class RightUIController
{
    public GameObject dragPanel;        // drag过程中的小洞背景

    private Color32 greenSliderColor = new Color32(0, 255, 11, 100);
    private Color32 yellowSliderColor = new Color32(255, 200, 0, 100);
    
    public GameObject skillPanel;       // 点击场上干员出现的右侧UI
    public Image rightSkillImage;       // 技能按钮图标     
    public Text spText;                 // 技能下方显示sp的Text
    public Slider colorSlider;          // 遮罩条，在技能图标上
    private Image colorSliderFill;      // colorSlider的填充图片

    // 攻击范围展示
    private List<GameObject> showingRangeImage = new List<GameObject>();

    
    public RightUIController()
    {
        dragPanel = OperUIElements.dragPanel;
        skillPanel = OperUIElements.skillPanel;
        rightSkillImage = OperUIElements.rightSkillImage;
        spText = OperUIElements.spText;
        colorSlider = OperUIElements.colorSlider;
        colorSliderFill = colorSlider.fillRect.GetComponent<Image>();
        
    }

    public void Update()
    {
        if (OperUIManager.sta != UIstate.UP) return;

        OperatorCore oc_ = OperUIManager.showingOper;
        rightSkillImage.sprite = oc_.od_.skillImage[oc_.skillNum];
        
        
        SPController sp_ = OperUIManager.showingOper.fightCalculation.sp_;
        if (!sp_.during)
        {   // 不在技能持续时间内，说明在技力积攒阶段
            colorSliderFill.color = greenSliderColor;
            colorSlider.value = sp_.sp / sp_.maxSp;
            if (sp_.sp < sp_.maxSp)
                spText.text = sp_.sp + "/" + sp_.maxSp;
            else
                spText.text = "READY";
        }
        else
        {   // 在技能持续时间内
            colorSliderFill.color = yellowSliderColor;
            colorSlider.value = sp_.remainingTime / sp_.maxTime;
            spText.text = "DURING";
        }
    }

    /// <summary>
    /// 根据OperUIManager的sta，设置该controller下成员
    /// </summary>
    public void SetBySta()
    {
        if (OperUIManager.sta == UIstate.Dragging)
        {
            dragPanel.SetActive(false);
            skillPanel.SetActive(false);
        }
        else if (OperUIManager.sta == UIstate.Down)
        {
            dragPanel.SetActive(false);
            skillPanel.SetActive(false);
        }
        else if (OperUIManager.sta == UIstate.UP)
        {
            dragPanel.SetActive(false);
            skillPanel.SetActive(true);
            ShowAtkRange(OperUIManager.showingOper);
        }
    }
    
    /// <summary>
    /// 刷新RightUI
    /// </summary>
    public void Refresh()
    {
        SetBySta();
    }

    /// <summary>
    /// 关闭UI前会调用的函数
    /// </summary>
    public void Close()
    {
        HideAtkRange();
    }
    
    /// <summary>
    /// 根据oper的pos，定位dragPanel的位置，并激活dragPanel
    /// </summary>
    /// <param name="operPos"></param>
    public void DragPanelPos(Vector3 operPos)
    {
        Vector3 holePos = Camera.main.WorldToScreenPoint(operPos);
        holePos.y += 50;
        dragPanel.transform.position = holePos;
        dragPanel.SetActive(true);
    }
    
    /// <summary>
    /// 在世界方块上展示传入的攻击范围，旋转和位置根据oper的旋转来
    /// </summary>
    public void ShowAtkRange(OperatorCore oc_, List<Vector2> posList)
    {
        HideAtkRange();
        
        float rol_y =oc_.atkRange.transform.rotation.eulerAngles.y;
        foreach (var detaPos in posList) 
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

    /// <summary>
    /// 默认展示oc_当前elitism等级的atkRange
    /// </summary>
    public void ShowAtkRange(OperatorCore oc_)
    {
        ShowAtkRange(oc_, oc_.atkRange.RangePos);
    }
    
}

public class LevelUIController
{
    public Image elitismImage;            // elitism部分
    public Animator elitismCostAnim;
    public Text elitismCostText;
    public Text elitismExpText;

    public Text operNameText;             // 名称部分
    public Image operImage;
    public Image elementImage;
    public Text operLevelText;
    public Button immediatelyButton;

    public Text atkText;                  // 属性部分
    public Text defText;
    public Text magicDefText;
    public Text blockText;
    public Text talentText;
    public Text lifeText;
    public Slider lifeSlider;

    public LevelUIController()
    {
        elitismImage = OperUIElements.elitismImage;
        elitismCostAnim = OperUIElements.elitismCostAnim;
        elitismCostText = OperUIElements.elitismCostText;
        elitismExpText = OperUIElements.elitismExpText;
        operNameText = OperUIElements.operNameText;
        operImage = OperUIElements.operImage;
        elementImage = OperUIElements.elementImage;
        operLevelText = OperUIElements.operLevelText;
        immediatelyButton = OperUIElements.immediatelyButton;
        atkText = OperUIElements.atkText;
        defText = OperUIElements.defText;
        magicDefText = OperUIElements.magicDefText;
        blockText = OperUIElements.blockText;
        talentText = OperUIElements.talentText;
        lifeText = OperUIElements.lifeText;
        lifeSlider = OperUIElements.lifeSlider;
    }

    public void Update()
    {
        RefreshUpdate();
    }

    /// <summary>
    /// 根据OperUIManager的sta，设置该controller下的成员
    /// </summary>
    public void SetBySta()
    {
        if (OperUIManager.sta == UIstate.Dragging)
        {
            immediatelyButton.gameObject.SetActive(false);
        }
        else if (OperUIManager.sta == UIstate.Down)
        {
            immediatelyButton.gameObject.SetActive(true);
        }
        else if (OperUIManager.sta == UIstate.UP)
        {
            immediatelyButton.gameObject.SetActive(false);
        }
        Refresh();
    }
    
    
    private void RefreshStart()
    {// 在UI打开或换人时调用一次
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        BattleCalculation bc_ = oc_.fightCalculation;

        if (oc_.eliteLevel == 0)
        {
            elitismImage.sprite = SpriteElement.elitismSprite0;
            elitismCostText.text = od_.elitismCost[0].ToString();
            elitismExpText.text = od_.elitismCost[0].ToString();
            operImage.sprite = od_.operUIImage1;
        }
        else if (oc_.eliteLevel == 1)
        {
            elitismImage.sprite = SpriteElement.elitismSprite1;
            elitismCostText.text = od_.elitismCost[1].ToString();
            elitismExpText.text = od_.elitismCost[1].ToString();
            operImage.sprite = od_.operUIImage1;
        }
        else if (oc_.eliteLevel == 2)
        {
            elitismImage.sprite = SpriteElement.elitismSprite2;
            operImage.sprite = od_.operUIImage2;
        }

        operNameText.text = od_.Name;
        elementImage.sprite = SpriteElement.GetElementSprite(od_.elementType);
        operLevelText.text = oc_.level + "/" + od_.maxLevel[oc_.eliteLevel];
        talentText.text = od_.Description[oc_.eliteLevel];
    }

    private void RefreshUpdate()
    {// 当UI打开时，每帧调用
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        BattleCalculation bc_ = oc_.fightCalculation;

        atkText.text = bc_.atk_.val.ToString(CultureInfo.InvariantCulture);
        defText.text = bc_.def_.val.ToString(CultureInfo.InvariantCulture);
        magicDefText.text = bc_.magicDef_.val.ToString(CultureInfo.InvariantCulture);
        blockText.text = bc_.maxBlock.ToString(CultureInfo.InvariantCulture);
        
        lifeText.text = bc_.life_.life + "/" + bc_.life_.val;
        lifeSlider.value = bc_.life_.life / bc_.life_.val;
    }

    /// <summary>
    /// 刷新LevelUIController的所有元素
    /// </summary>
    public void Refresh()
    {
        RefreshStart();
        RefreshUpdate();
    }

}

public class SkillUIController
{
    private Image skill_1_ButtonImage;
    private Image skill_2_ButtonImage;
    private Image skill_3_ButtonImage;
    private Text skill_1_ButtonText;
    private Text skill_2_ButtonText;
    private Text skill_3_ButtonText;
    private Image detailedValuesButtonImage;
    private Image detailedTalentButtonImage;
    private Text elementalMasteryText;
    private Text elementalDamageText;
    private Text elementalResistanceText;
    private Text spRechargeText;
    private Text currentBlockText;
    private Text atkSpeedText;
    private Text minAtkInterval;
    private Text talent1;
    private Text talent2;
    private GameObject detailedValuesObject;
    private GameObject detailedTalentObject;
    private GameObject skillObject;
    private GameObject skillLockObject;
    private Image skillImage;
    private Image skillChooseImage;
    private Text skillChooseText;
    private Button skillChooseButton;
    private Text skillLevelText;
    private Image skillLevelUpImage;
    private Text skillLevelUpText;
    private Button skillLevelUpButton;
    private GameObject skillResource;
    private Text skillCostText;
    private Text skillExpText;
    private Text skillName;
    private Image recoveryTypeImage;
    private Text recoveryTypeText;
    private Image triggerTypeImage;
    private Text triggerTypeText;
    private GameObject durationObject;
    private Text durationText;
    private GameObject spObject;
    private Text beginSpText;
    private Text maxSpText;
    private Text skillDescriptionText;

    
    private Color32 skillTextChooseColor = new Color32(50, 220, 50, 255);
    private Color32 buttonChooseColor = new Color32(0, 0, 0, 255);
    private Color32 buttonUnChooseColor = new Color32(120, 120, 120, 255);
    
    
    
    public SkillUISta skillUISta;


    public SkillUIController()
    {
        skill_1_ButtonImage = OperUIElements.skill_1_ButtonImage;
        skill_2_ButtonImage = OperUIElements.skill_2_ButtonImage;
        skill_3_ButtonImage = OperUIElements.skill_3_ButtonImage;
        skill_1_ButtonText = OperUIElements.skill_1_ButtonText;
        skill_2_ButtonText = OperUIElements.skill_2_ButtonText;
        skill_3_ButtonText = OperUIElements.skill_3_ButtonText;
        detailedValuesButtonImage = OperUIElements.detailedValuesButtonImage;
        detailedTalentButtonImage = OperUIElements.detailedTalentButtonImage;
        elementalMasteryText = OperUIElements.elementalMasteryText;
        elementalDamageText = OperUIElements.elementalDamageText;
        elementalResistanceText = OperUIElements.elementalResistanceText;
        spRechargeText = OperUIElements.spRechargeText;
        currentBlockText = OperUIElements.currentBlockText;
        atkSpeedText = OperUIElements.atkSpeedText;
        minAtkInterval = OperUIElements.minAtkInterval;
        talent1 = OperUIElements.talent1;
        talent2 = OperUIElements.talent2;
        detailedValuesObject = OperUIElements.detailedValuesObject;
        detailedTalentObject = OperUIElements.detailedTalentObject;
        skillObject = OperUIElements.skillObject;
        skillLockObject = OperUIElements.skillLockObject;
        skillImage = OperUIElements.skillImage;
        skillChooseImage = OperUIElements.skillChooseImage;
        skillChooseText = OperUIElements.skillChooseText;
        skillChooseButton = OperUIElements.skillChooseButton;
        skillLevelText = OperUIElements.skillLevelText;
        skillLevelUpImage = OperUIElements.skillLevelUpImage;
        skillLevelUpText = OperUIElements.skillLevelUpText;
        skillLevelUpButton = OperUIElements.skillLevelUpButton;
        skillResource = OperUIElements.skillResource;
        skillCostText = OperUIElements.skillCostText;
        skillExpText = OperUIElements.skillExpText;
        skillName = OperUIElements.skillName;
        recoveryTypeImage = OperUIElements.recoveryTypeImage;
        recoveryTypeText = OperUIElements.recoveryTypeText;
        triggerTypeImage = OperUIElements.triggerTypeImage;
        triggerTypeText = OperUIElements.triggerTypeText;
        durationObject = OperUIElements.durationObject;
        durationText = OperUIElements.durationText;
        spObject = OperUIElements.spObject;
        beginSpText = OperUIElements.beginSpText;
        maxSpText = OperUIElements.maxSpText;
        skillDescriptionText = OperUIElements.skillDescriptionText;
    }

    private void SetStaBySkillNum()
    {// 根据自身选择的skillNum，更新skillUISta
        skill_1_ButtonText.color=Color.white;
        skill_2_ButtonText.color=Color.white;
        skill_3_ButtonText.color=Color.white;
        
        switch (OperUIManager.showingOper.skillNum)
        {
            case 0:
                skill_1_ButtonText.color = skillTextChooseColor;
                skillUISta = SkillUISta.skill1;
                break;
            case 1:
                skill_2_ButtonText.color = skillTextChooseColor;
                skillUISta = SkillUISta.skill2;
                break;
            case 2:
                skill_3_ButtonText.color = skillTextChooseColor;
                skillUISta = SkillUISta.skill3;
                break;
            default:
                skill_1_ButtonText.color = skillTextChooseColor;
                skillUISta = SkillUISta.skill1;
                break;
        }
    }
    
    public void SetBySta()
    {
        SetStaBySkillNum();
        RefreshBySta();
    }

    public void Refresh()
    {
        RefreshBySta();
        RefreshContents();
    }

    private void RefreshBySta()
    {// 根据自身sta激活对应的面板
        detailedValuesObject.SetActive(false);
        detailedTalentObject.SetActive(false);
        skillObject.SetActive(false);
        skillLockObject.SetActive(false);

        detailedValuesButtonImage.color = buttonUnChooseColor;
        detailedTalentButtonImage.color = buttonUnChooseColor;
        skill_1_ButtonImage.color = buttonUnChooseColor;
        skill_2_ButtonImage.color = buttonUnChooseColor;
        skill_3_ButtonImage.color = buttonUnChooseColor;

        switch (skillUISta)
        {
            case SkillUISta.detailedValue:
                detailedValuesObject.SetActive(true);
                detailedValuesButtonImage.color = buttonChooseColor;
                break;
            case SkillUISta.detailedTalent:
                detailedTalentObject.SetActive(true);
                detailedTalentButtonImage.color = buttonChooseColor;
                break;
            case SkillUISta.skill1:
                skillObject.SetActive(true);
                skill_1_ButtonImage.color = buttonChooseColor;
                break;
            case SkillUISta.skill2:
                skillObject.SetActive(true);
                skill_2_ButtonImage.color = buttonChooseColor;
                break;
            case SkillUISta.skill3:
                skillObject.SetActive(true);
                skill_3_ButtonImage.color = buttonChooseColor;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        RefreshContents();
    }

    private void RefreshContents()
    {
        // elementalMasteryText=
        // elementalDamageText;
        // elementalResistanceText;
        // spRechargeText;
        // currentBlockText;
        // atkSpeedText;
        // minAtkInterval;
        // talent1;
        // talent2;
    }
    
    
}

public enum SkillUISta
{
    skill1,
    skill2,
    skill3,
    detailedValue,
    detailedTalent
}