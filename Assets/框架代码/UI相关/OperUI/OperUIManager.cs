using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

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
    public static EdgeUIController edgeUIController;
    
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
        edgeUIController = new EdgeUIController();
    }

    private void Update()
    {
        edgeUIController.Update();
        
        if (!showing) return;
        rightUIController.Update();
        levelUIController.Update();
    }

    /// <summary>
    /// 每关开始Start时调用
    /// </summary>
    public static void Init()
    {
        edgeUIController.Init();
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
        if (showingOper == null) return;
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
        
        
        SPController sp_ = OperUIManager.showingOper.sp_;
        if (!sp_.during)
        {   // 不在技能持续时间内，说明在技力积攒阶段
            colorSliderFill.color = greenSliderColor;
            colorSlider.value = sp_.sp / sp_.maxSp;
            if (sp_.sp < sp_.maxSp)
            {
                colorSlider.gameObject.SetActive(true);
                spText.text = sp_.sp.ToString("f0") + "/" + sp_.maxSp;
            }
            else
            {
                colorSlider.gameObject.SetActive(false);
                spText.text = "READY";
            }
        }
        else
        {   // 在技能持续时间内
            colorSlider.gameObject.SetActive(true);
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
            
            GameObject showing = PoolManager.GetObj(StoreHouse.instance.atkRangeImage);
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

        if (oc_.eliteLevel == 0)
        {
            elitismImage.sprite = StoreHouse.instance.elitismSprite0;
            elitismCostText.text = od_.elitismCost[0] < 1e5 ? od_.elitismCost[0].ToString() : "无效";
            elitismExpText.text = od_.elitismExp[0] < 1e5 ? od_.elitismExp[0].ToString() : "无效";
            operImage.sprite = od_.operUIImage1;
        }
        else if (oc_.eliteLevel == 1)
        {
            elitismImage.sprite = StoreHouse.instance.elitismSprite1;
            elitismCostText.text = od_.elitismCost[1] < 1e5 ? od_.elitismCost[1].ToString() : "无效";
            elitismExpText.text = od_.elitismExp[1] < 1e5 ? od_.elitismExp[1].ToString() : "无效";
            operImage.sprite = od_.operUIImage1;
        }
        else if (oc_.eliteLevel == 2)
        {
            elitismImage.sprite = StoreHouse.instance.elitismSprite2;
            operImage.sprite = od_.operUIImage2;
        }

        operNameText.text = od_.Name;
        elementImage.sprite = StoreHouse.GetElementSprite(od_.elementType);
        operLevelText.text = oc_.level + "/" + od_.maxLevel[oc_.eliteLevel];
        talentText.text = od_.Description[oc_.eliteLevel];
    }

    private void RefreshUpdate()
    {// 当UI打开时，每帧调用
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;

        atkText.text = oc_.atk_.val.ToString(CultureInfo.InvariantCulture);
        defText.text = oc_.def_.val.ToString(CultureInfo.InvariantCulture);
        magicDefText.text = oc_.magicDef_.val.ToString(CultureInfo.InvariantCulture);
        blockText.text = ((int)oc_.maxBlock.val).ToString(CultureInfo.InvariantCulture);
        
        lifeText.text = oc_.life_.life + "/" + oc_.life_.val;
        lifeSlider.value = oc_.life_.life / oc_.life_.val;
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
    
    private Text atkDetailText;
    private Text defDetailText;
    private Text magicDefDetailText;
    private Text blockDetailText;
    private Text lifeDetailText;
    private Text elementalMasteryText;
    private Text elementalDamageText;
    private Text elementalResistanceText;
    private Text spRechargeText;
    private Text costDetailText;
    private Text reTimeDetailText;
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

    private Color32 skillCanChooseColor = new Color32(0, 200, 0, 150);
    private Color32 skillIsChoosedColor = new Color32(150, 150, 150, 150);
    private Color32 skillCanLevelUpColor = new Color32(0, 200, 200, 150);
    private Color32 skillMaxLevelUpColor = new Color32(150, 150, 150, 150);
    
    
    public SkillUISta skillUISta;
    public int showSkillNum { get; private set; }

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
        
        atkDetailText = OperUIElements.atkDetailText;
        defDetailText = OperUIElements.defDetailText;
        magicDefDetailText = OperUIElements.magicDefDetailText;
        blockDetailText = OperUIElements.blockDetailText;
        lifeDetailText = OperUIElements.lifeDetailText;
        elementalMasteryText = OperUIElements.elementalMasteryText;
        elementalDamageText = OperUIElements.elementalDamageText;
        elementalResistanceText = OperUIElements.elementalResistanceText;
        spRechargeText = OperUIElements.spRechargeText;
        costDetailText = OperUIElements.costDetailText;
        reTimeDetailText = OperUIElements.reTimeDetailText;
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
    {
        // 根据自身选择的skillNum，更新skillUISta
        skillUISta = OperUIManager.showingOper.skillNum switch
        {
            0 => SkillUISta.skill1,
            1 => SkillUISta.skill2,
            2 => SkillUISta.skill3,
            _ => skillUISta
        };
    }

    private void SetSkillButtonColorBySkillNum()
    {
        skill_1_ButtonText.color = Color.white;
        skill_2_ButtonText.color = Color.white;
        skill_3_ButtonText.color = Color.white;
        switch (OperUIManager.showingOper.skillNum)
        {
            case 0:
                skill_1_ButtonText.color = skillTextChooseColor;
                break;
            case 1:
                skill_2_ButtonText.color = skillTextChooseColor;
                break;
            case 2:
                skill_3_ButtonText.color = skillTextChooseColor;
                break;
        }
    }
    
    public void SetBySta()
    {
        SetStaBySkillNum();
        SetSkillButtonColorBySkillNum();
        RefreshBySta();
    }

    public void Refresh()
    {
        RefreshBySta();
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

        int eliteLevel = OperUIManager.showingOper.eliteLevel;
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
                skillLockObject.SetActive(false);
                skill_1_ButtonImage.color = buttonChooseColor;
                break;
            case SkillUISta.skill2:
                if (eliteLevel < 1)
                {
                    skillObject.SetActive(false);
                    skillLockObject.SetActive(true);
                }
                else
                {
                    skillObject.SetActive(true);
                    skillLockObject.SetActive(false);
                }
                skill_2_ButtonImage.color = buttonChooseColor;
                break;
            case SkillUISta.skill3:
                if (eliteLevel < 2)
                {
                    skillObject.SetActive(false);
                    skillLockObject.SetActive(true);
                }
                else
                {
                    skillObject.SetActive(true);
                    skillLockObject.SetActive(false);
                }
                skill_3_ButtonImage.color = buttonChooseColor;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        RefreshContents();
    }

    private Color GetRecoverTypeColor(recoverType type)
    {
        return type switch
        {
            recoverType.auto => new Color32(12, 193, 94, 255),
            recoverType.atk => new Color32(230, 36, 58, 255),
            recoverType.beAtk => new Color32(255, 162, 28, 255),
            _ => Color.black
        };
    }

    private string GetRecoverTypeText(recoverType type)
    {
        return type switch
        {
            recoverType.auto => "自动回复",
            recoverType.atk => "攻击回复",
            recoverType.beAtk => "受击回复",
            _ => "?!?!"
        };
    }

    private string GetReleaseTypeText(releaseType type)
    {
        return type switch
        {
            releaseType.hand => "手动触发",
            releaseType.auto => "自动触发",
            releaseType.atk => "攻击触发",
            releaseType.passive => "被动技能",
            _ => "??!!"
        };
    }

    string GreenString(string s)
    {
        return "<color=\"#00FF00\">" + s + "</color>";
    }
    
    string RedString(string s)
    {
        return "<color=\"#FF0000\">" + s + "</color>";
    }

    string ToDetailString(float baseVal, float val, int places, bool incRed = false, bool percentage = false)
    {
        int tmp = (int) Math.Pow(10, places);
        float deta = val - baseVal;
        string percent = percentage ? "%" : "";
        baseVal = (float) ((int) (baseVal * tmp)) / (float) tmp;
        deta = (float) ((int) (deta * tmp)) / (float) tmp;
        if (percentage)
        {
            baseVal *= 100;
            deta *= 100;
        }
        if (Mathf.Abs(deta) < 1e-3) return baseVal.ToString(CultureInfo.InvariantCulture) + percent;
        
        bool isGreen = (deta > 0) ^ incRed;
        string symbol = deta > 0 ? "+" : "";
        if (isGreen)
            return baseVal.ToString(CultureInfo.InvariantCulture) + percent +
                   GreenString(symbol + deta.ToString(CultureInfo.InvariantCulture) + percent);
        else
            return baseVal.ToString(CultureInfo.InvariantCulture) + percent +
                   RedString(symbol + deta.ToString(CultureInfo.InvariantCulture) + percent);
    }

    string ToDetailString(ValueBuffer buffer, int places = 0, bool incRed = false, bool percentage = false)
    {
        return ToDetailString(buffer.baseVal, buffer.val, places, incRed, percentage);
    }
    
    private void RefreshContents()
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        int skillNum = oc_.skillNum;
        showSkillNum = skillUISta switch
        {
            SkillUISta.skill1 => 0,
            SkillUISta.skill2 => 1,
            SkillUISta.skill3 => 2,
            _ => 0
        };
        int skillLevel = oc_.skillLevel[showSkillNum];

        // 刷新已选择技能文字的颜色
        SetSkillButtonColorBySkillNum();

        // 刷新详细数据与详细天赋
        atkDetailText.text = ToDetailString(oc_.atk_);
        defDetailText.text = ToDetailString(oc_.def_);
        magicDefDetailText.text = ToDetailString(oc_.magicDef_);
        blockDetailText.text = ToDetailString(oc_.maxBlock);
        lifeDetailText.text = ToDetailString(oc_.life_);

        elementalMasteryText.text = ToDetailString(oc_.elementMastery);
        elementalDamageText.text = ToDetailString(oc_.elementDamage, 2, false, true);
        elementalResistanceText.text = ToDetailString(oc_.elementResistance, 2, false, true);
        spRechargeText.text = ToDetailString(oc_.sp_.spRecharge, 2, false, true);

        costDetailText.text = ToDetailString(oc_.costNeed, 0, true);
        reTimeDetailText.text = ToDetailString(oc_.recoverTime, 0, true);

        int atkSpeed = (int) oc_.atkSpeedController.atkSpeed.val;
        atkSpeedText.text = atkSpeed == 0 ? "0" :
            atkSpeed > 0 ? GreenString(atkSpeed.ToString()) : RedString(atkSpeed.ToString());
        minAtkInterval.text = ToDetailString(oc_.atkSpeedController.baseInterval,
            oc_.atkSpeedController.minAtkInterval, 2, true);

        talent1.text = oc_.GetTalentDescription(1);
        talent2.text = oc_.GetTalentDescription(2);
        
        // 刷新技能图标
        skillImage.sprite = od_.skillImage[showSkillNum];
        // 刷新技能按钮
        if (skillNum == showSkillNum)
        {
            skillChooseImage.color = skillIsChoosedColor;
            skillChooseText.text = "已选择";
        }
        else
        {
            skillChooseImage.color = skillCanChooseColor;
            skillChooseText.text = "选择";
        }
        
        // 根据当前选择的技能，刷新描述界面
        switch (showSkillNum)
        {
            case 0:
                skillCostText.text = od_.costNeed0[Math.Min(skillLevel, 5)].ToString();
                skillExpText.text = od_.expNeed0[Math.Min(skillLevel, 5)].ToString();
                skillName.text = od_.skillName0;
                recoveryTypeImage.color = GetRecoverTypeColor(od_.skill0_recoverType);
                recoveryTypeText.text = GetRecoverTypeText(od_.skill0_recoverType);
                triggerTypeText.text = GetReleaseTypeText(od_.skill0_releaseType);
                durationText.text = od_.duration0[skillLevel].ToString("f0");
                beginSpText.text = od_.initSP0[skillLevel].ToString();
                maxSpText.text = od_.maxSP0[skillLevel].ToString();
                skillDescriptionText.text = oc_.GetSkillDescription(0);
                break;
            case 1:
                skillCostText.text = od_.costNeed1[Math.Min(skillLevel, 5)].ToString();
                skillExpText.text = od_.expNeed1[Math.Min(skillLevel, 5)].ToString();
                skillName.text = od_.skillName1;
                recoveryTypeImage.color = GetRecoverTypeColor(od_.skill1_recoverType);
                recoveryTypeText.text = GetRecoverTypeText(od_.skill1_recoverType);
                triggerTypeText.text = GetReleaseTypeText(od_.skill1_releaseType);
                durationText.text = od_.duration1[skillLevel].ToString("f0");
                beginSpText.text = od_.initSP1[skillLevel].ToString();
                maxSpText.text = od_.maxSP1[skillLevel].ToString();
                skillDescriptionText.text = oc_.GetSkillDescription(1);
                break;
            case 2:
                skillCostText.text = od_.costNeed2[Math.Min(skillLevel, 5)].ToString();
                skillExpText.text = od_.expNeed2[Math.Min(skillLevel, 5)].ToString();
                skillName.text = od_.skillName2;
                recoveryTypeImage.color = GetRecoverTypeColor(od_.skill2_recoverType);
                recoveryTypeText.text = GetRecoverTypeText(od_.skill2_recoverType);
                triggerTypeText.text = GetReleaseTypeText(od_.skill2_releaseType);
                durationText.text = od_.duration2[skillLevel].ToString("f0");
                beginSpText.text = od_.initSP2[skillLevel].ToString();
                maxSpText.text = od_.maxSP2[skillLevel].ToString();
                skillDescriptionText.text = oc_.GetSkillDescription(2);
                break;
        }

        // 刷新一些其他技能描述界面
        skillLevelText.text = (skillLevel + 1).ToString();
        if (skillLevel < 6)
        {
            skillLevelUpImage.color = skillCanLevelUpColor;
            skillLevelUpText.text = "升级";
            skillResource.SetActive(true);
        }
        else
        {
            skillLevelUpImage.color = skillMaxLevelUpColor;
            skillLevelUpText.text = "已满级";
            skillResource.SetActive(false);
        }
        
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

public class EdgeUIController
{
    public Text expText;
    public Text costText;
    public Slider costSlider;
    public Text remainPlaceText;
    public Text waveText;
    public Text levelHPText;

    public Image globalSpeedImage;
    public Image globalPauseImage;
    public Image settingImage;

    private float costDetaTime;
    
    public EdgeUIController()
    {
        expText = OperUIElements.expText;
        costText = OperUIElements.costText;
        costSlider = OperUIElements.costSlider;
        remainPlaceText = OperUIElements.remainPlaceText;
        waveText = OperUIElements.waveText;
        levelHPText = OperUIElements.levelHPText;
        globalSpeedImage = OperUIElements.globalSpeedImage;
        globalPauseImage = OperUIElements.globalPauseImage;
        settingImage = OperUIElements.settingImage;
    }

    public void Init()
    {
        costDetaTime = 0;
    }

    public void Update()
    {
        AutoGetCost();
        Refresh();
    }

    
    private void AutoGetCost()
    {
        costDetaTime += Time.deltaTime;
        if (costDetaTime >= 1)
        {
            costDetaTime = 0;
            InitManager.resourceController.CostIncrease(1);
        }
    }

    private void Refresh()
    {
        if (OperUIManager.showing) settingImage.gameObject.SetActive(false);
        else settingImage.gameObject.SetActive(true);

        expText.text = InitManager.resourceController.exp.ToString("f0");
        costText.text = InitManager.resourceController.cost.ToString("f0");
        costSlider.value = costDetaTime;
        remainPlaceText.text = InitManager.resourceController.remainPlace.ToString();
        waveText.text = (InitManager.enemyWaveController.wave + 1) + "/" +
                        InitManager.enemyWaveController.maxWave;
        levelHPText.text = InitManager.resourceController.HP.ToString();


        globalPauseImage.sprite = InitManager.globalPause
            ? StoreHouse.instance.continueSprite
            : StoreHouse.instance.pauseSprite;
        globalSpeedImage.sprite=InitManager.globalDoubleSpeed
            ? StoreHouse.instance.speed2x_Sprite
            : StoreHouse.instance.speed1x_Sprite;
    }


}

