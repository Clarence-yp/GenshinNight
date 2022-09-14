using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperUIElements : MonoBehaviour
{
    private static OperUIElements instance;

    [Header("rightUI相关")]
    public GameObject dragPanel_p;
    public static GameObject dragPanel;
    public GameObject skillPanel_p;
    public static GameObject skillPanel;
    public Image rightSkillImage_p;
    public static Image rightSkillImage;
    public Text spText_p;
    public static Text spText;
    public Slider colorSlider_p;
    public static Slider colorSlider;
    
    [Header("LevelUI相关")]
    public Image elitismImage_p;            // elitism部分
    public static Image elitismImage;
    public Animator elitismCostAnim_p;
    public static Animator elitismCostAnim;
    public Text elitismCostText_p;
    public static Text elitismCostText;
    public Text elitismExpText_p;
    public static Text elitismExpText;

    public Text operNameText_p;             // 名称部分
    public static Text operNameText;
    public Image operImage_p;
    public static Image operImage;
    public Image elementImage_p;
    public static Image elementImage;
    public Text operLevelText_p;
    public static Text operLevelText;
    public Button immediatelyButton_p;
    public static Button immediatelyButton;
    
    public Text atkText_p;                  // 属性部分
    public static Text atkText;
    public Text defText_p;
    public static Text defText;
    public Text magicDefText_p;
    public static Text magicDefText;
    public Text blockText_p;
    public static Text blockText;
    public Text talentText_p;
    public static Text talentText;
    public Text lifeText_p;
    public static Text lifeText;
    public Slider lifeSlider_p;
    public static Slider lifeSlider;

    [Header("SkillUI相关")] 
    public Image skill_1_ButtonImage_p;         // 选择按钮部分
    public static Image skill_1_ButtonImage;
    public Image skill_2_ButtonImage_p;
    public static Image skill_2_ButtonImage;
    public Image skill_3_ButtonImage_p;
    public static Image skill_3_ButtonImage;
    public Text skill_1_ButtonText_p;
    public static Text skill_1_ButtonText;
    public Text skill_2_ButtonText_p;
    public static Text skill_2_ButtonText;
    public Text skill_3_ButtonText_p;
    public static Text skill_3_ButtonText;
    public Image detailedValuesButtonImage_p;
    public static Image detailedValuesButtonImage;
    public Image detailedTalentButtonImage_p;
    public static Image detailedTalentButtonImage;

    public Text atkDetailText_p;                // 详细数值/天赋部分
    public static Text atkDetailText;
    public Text defDetailText_p;
    public static Text defDetailText;
    public Text magicDefDetailText_p;
    public static Text magicDefDetailText;
    public Text blockDetailText_p;
    public static Text blockDetailText;
    public Text lifeDetailText_p;
    public static Text lifeDetailText;
    public Text elementalMasteryText_p;     
    public static Text elementalMasteryText;
    public Text elementalDamageText_p;
    public static Text elementalDamageText;
    public Text elementalResistanceText_p;
    public static Text elementalResistanceText;
    public Text spRechargeText_p;
    public static Text spRechargeText;
    public Text costDetailText_p;
    public static Text costDetailText;
    public Text reTimeDetailText_p;
    public static Text reTimeDetailText;
    public Text atkSpeedText_p;
    public static Text atkSpeedText;
    public Text minAtkInterval_p;
    public static Text minAtkInterval;
    public Text talent1_p;
    public static Text talent1;
    public Text talent2_p;
    public static Text talent2;
    public GameObject detailedValuesObject_p;
    public static GameObject detailedValuesObject;
    public GameObject detailedTalentObject_p;
    public static GameObject detailedTalentObject;
    public GameObject skillObject_p;
    public static GameObject skillObject;
    public GameObject skillLockObject_p;
    public static GameObject skillLockObject;

    public Image skillImage_p;              // 技能左侧部分
    public static Image skillImage;
    public Image skillChooseImage_p;
    public static Image skillChooseImage;
    public Text skillChooseText_p;
    public static Text skillChooseText;
    public Button skillChooseButton_p;
    public static Button skillChooseButton;
    public Text skillLevelText_p;
    public static Text skillLevelText;
    public Image skillLevelUpImage_p;
    public static Image skillLevelUpImage;
    public Text skillLevelUpText_p;
    public static Text skillLevelUpText;
    public Button skillLevelUpButton_p;
    public static Button skillLevelUpButton;
    public GameObject skillResource_p;
    public static GameObject skillResource;
    public Text skillCostText_p;
    public static Text skillCostText;
    public Text skillExpText_p;
    public static Text skillExpText;

    public Text skillName_p;                // 技能右侧部分
    public static Text skillName;
    public Image recoveryTypeImage_p;
    public static Image recoveryTypeImage;
    public Text recoveryTypeText_p;
    public static Text recoveryTypeText;
    public Image triggerTypeImage_p;
    public static Image triggerTypeImage;
    public Text triggerTypeText_p;
    public static Text triggerTypeText;
    public GameObject durationObject_p; 
    public static GameObject durationObject; 
    public Text durationText_p;
    public static Text durationText;
    public GameObject spObject_p;
    public static GameObject spObject;
    public Text beginSpText_p;
    public static Text beginSpText;
    public Text maxSpText_p;
    public static Text maxSpText;
    public Text skillDescriptionText_p;
    public static Text skillDescriptionText;

    [Header("EdgeUI相关")] 
    public Text expText_p;
    public static Text expText;
    public Text costText_p;
    public static Text costText;
    public Slider costSlider_p;
    public static Slider costSlider;
    public Text remainPlaceText_p;
    public static Text remainPlaceText;
    public Text waveText_p;
    public static Text waveText;
    public Text levelHPText_p;
    public static Text levelHPText;
    public Image globalSpeedImage_p;
    public static Image globalSpeedImage;
    public Image globalPauseImage_p;
    public static Image globalPauseImage;
    public Image settingImage_p;
    public static Image settingImage;
    

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        
        Init();
    }
    
    private void Init()
    {
        // rightUI相关
        dragPanel = dragPanel_p;
        skillPanel = skillPanel_p;
        rightSkillImage = rightSkillImage_p;
        spText = spText_p;
        colorSlider = colorSlider_p;

        // LevelUI相关
        elitismImage = elitismImage_p;
        elitismCostAnim = elitismCostAnim_p;
        elitismCostText = elitismCostText_p;
        elitismExpText = elitismExpText_p;
        operNameText = operNameText_p;
        operImage = operImage_p;
        elementImage = elementImage_p;
        operLevelText = operLevelText_p;
        immediatelyButton = immediatelyButton_p;
        atkText = atkText_p;
        defText = defText_p;
        magicDefText = magicDefText_p;
        blockText = blockText_p;
        talentText = talentText_p;
        lifeText = lifeText_p;
        lifeSlider = lifeSlider_p;
        
        // skillUI相关
        skill_1_ButtonImage = skill_1_ButtonImage_p;
        skill_2_ButtonImage = skill_2_ButtonImage_p;
        skill_3_ButtonImage = skill_3_ButtonImage_p;
        skill_1_ButtonText = skill_1_ButtonText_p;
        skill_2_ButtonText = skill_2_ButtonText_p;
        skill_3_ButtonText = skill_3_ButtonText_p;
        detailedValuesButtonImage = detailedValuesButtonImage_p;
        detailedTalentButtonImage = detailedTalentButtonImage_p;

        atkDetailText = atkDetailText_p;
        defDetailText = defDetailText_p;
        magicDefDetailText = magicDefDetailText_p;
        blockDetailText = blockDetailText_p;
        lifeDetailText = lifeDetailText_p;
        elementalMasteryText = elementalMasteryText_p;
        elementalDamageText = elementalDamageText_p;
        elementalResistanceText = elementalResistanceText_p;
        spRechargeText = spRechargeText_p;
        costDetailText = costDetailText_p;
        reTimeDetailText = reTimeDetailText_p;
        atkSpeedText = atkSpeedText_p;
        minAtkInterval = minAtkInterval_p;
        talent1 = talent1_p;
        talent2 = talent2_p;
        detailedValuesObject=detailedValuesObject_p;
        detailedTalentObject=detailedTalentObject_p;
        skillObject=skillObject_p;
        skillLockObject=skillLockObject_p;

        skillImage = skillImage_p;
        skillChooseImage = skillChooseImage_p;
        skillChooseText = skillChooseText_p;
        skillChooseButton = skillChooseButton_p;
        skillLevelText = skillLevelText_p;
        skillLevelUpImage = skillLevelUpImage_p;
        skillLevelUpText = skillLevelUpText_p;
        skillLevelUpButton = skillLevelUpButton_p;
        skillResource = skillResource_p;
        skillCostText = skillCostText_p;
        skillExpText = skillExpText_p;

        skillName = skillName_p;
        recoveryTypeImage = recoveryTypeImage_p;
        recoveryTypeText = recoveryTypeText_p;
        triggerTypeImage = triggerTypeImage_p;
        triggerTypeText = triggerTypeText_p;
        durationObject=durationObject_p;
        durationText = durationText_p;
        spObject = spObject_p;
        beginSpText = beginSpText_p;
        maxSpText = maxSpText_p;
        skillDescriptionText = skillDescriptionText_p;

        // EdgeUI相关
        expText = expText_p;
        costText = costText_p;
        costSlider = costSlider_p;
        remainPlaceText = remainPlaceText_p;
        waveText = waveText_p;
        levelHPText = levelHPText_p;
        globalSpeedImage = globalSpeedImage_p;
        globalPauseImage = globalPauseImage_p;
        settingImage = settingImage_p;
    }
    
}
