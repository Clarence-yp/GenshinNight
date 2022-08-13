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
        elementImage = elementImage_p;
        operLevelText = operLevelText_p;
        immediatelyButton = immediatelyButton_p;
        atkText = atkText_p;
        defText = defText_p;
        magicDefText = magicDefText_p;
        blockText = blockText_p;
        lifeText = lifeText_p;
        lifeSlider = lifeSlider_p;
        
        



        
    }
    
}
