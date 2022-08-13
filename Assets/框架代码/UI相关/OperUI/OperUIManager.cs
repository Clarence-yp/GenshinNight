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

    private static gradualChange gc_;

    // public GameObject leftUI;
    public static RightUIController rightUIController;
    public static LevelUIController levelUIController;
    
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

    private static void StartAllController()
    {
        // 使用sta初始化所有controller
        rightUIController.SetBySta();
        levelUIController.SetBySta();
        
        // 刷新Controller
        levelUIController.Refresh();
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
        
        
        SPController sp_ = OperUIManager.showingOper.battleCalculation.sp_;
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
    /// 根据OperUIManager的sta，设置该controller下成员的激活
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
        }
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
        elementImage = OperUIElements.elementImage;
        operLevelText = OperUIElements.operLevelText;
        immediatelyButton = OperUIElements.immediatelyButton;
        atkText = OperUIElements.atkText;
        defText = OperUIElements.defText;
        magicDefText = OperUIElements.magicDefText;
        blockText = OperUIElements.blockText;
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
    }
    
    
    private void RefreshStart()
    {// 在UI打开或换人时调用一次
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        BattleCalculation bc_ = oc_.battleCalculation;

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
        BattleCalculation bc_ = oc_.battleCalculation;

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