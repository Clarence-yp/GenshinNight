using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreHouse : MonoBehaviour
{
    public static StoreHouse instance;
    
    [Header("OperUI相关")]
    public Sprite elitismSprite0;
    public Sprite elitismSprite1;
    public Sprite elitismSprite2;
    public Sprite anemoSprite;
    public Sprite geoSprite;
    public Sprite electroSprite;
    public Sprite dendroSprite;
    public Sprite hydroSprite;
    public Sprite pyroSprite;
    public Sprite cryoSprite;

    [Header("EdgeUI相关")] 
    public Sprite speed1x_Sprite;
    public Sprite speed2x_Sprite;
    public Sprite pauseSprite;
    public Sprite continueSprite;
    
    
    [Header("干员放置相关")]
    public GameObject atkRangeImage; // 干员的攻击范围展示条纹图片

    [Header("伤害数字等显示")] 
    public GameObject reactionShowText;
    public GameObject smallDamageText;
    public GameObject bigDamageText;

    [Header("元素反应相关")] 
    public GameObject overLoadAnim;
    public GameObject superConductAnim;
    public GameObject superConductDuration;
    public GameObject electroChargedAnim;
    public GameObject swirlPyroAnim;
    public GameObject swirlHydroAnim;
    public GameObject swirlElectroAnim;
    public GameObject swirlCryoAnim;
    public GameObject crystallizationPyroShield;
    public GameObject crystallizationHydroShield;
    public GameObject crystallizationElectroShield;
    public GameObject crystallizationCryoShield;
    
    
    public static Color32 OverLoadTextColor = new Color32(233, 54, 64, 255);
    public static Color32 SuperConductTextColor = new Color32(221, 196, 255, 255);
    public static Color32 ElectroChargedColor = new Color32(186, 47, 255, 255);
    public static Color32 FrozenColor = new Color32(52, 192, 255, 255);
    public static Color32 VaporizeColor = new Color32(255, 150, 100, 255);
    public static Color32 MeltColor = new Color32(255, 150, 150, 255);
    public static Color32 SwirlColor = new Color32(50, 255, 200, 255);
    public static Color32 CrystallizationColor = new Color32(255, 180, 0, 255);


    public static Color32 AnemoDamageColor = new Color32(0, 255, 200, 255);
    public static Color32 GeoDamageColor = new Color32(255, 200, 0, 255);
    public static Color32 ElectroDamageColor = new Color32(200, 50, 255, 255);
    public static Color32 DendroDamageColor = new Color32(0, 175, 0, 255);
    public static Color32 HydroDamageColor = new Color32(0, 160, 255, 255);
    public static Color32 PyroDamageColor = new Color32(255, 50, 50, 255);
    public static Color32 CryoDamageColor = new Color32(175, 255, 255, 255);

    [Header("buff持续相关")] 
    public GameObject underGroundLight;



    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    

    public static Sprite GetElementSprite(ElementType type)
    {
        switch (type)
        {
            case ElementType.Anemo:
                return instance.anemoSprite;
            case ElementType.Geo:
                return instance.geoSprite;
            case ElementType.Electro:
                return instance.electroSprite;
            case ElementType.Dendro:
                return instance.dendroSprite;
            case ElementType.Hydro:
                return instance.hydroSprite;
            case ElementType.Pyro:
                return instance.pyroSprite;
            case ElementType.Cryo:
                return instance.cryoSprite;
            default:
                return null;
        }
    }

    public static GameObject GetSwirlAnim(ElementType type)
    {
        switch (type)
        {
            case ElementType.Electro:
                return instance.swirlElectroAnim;
            case ElementType.Hydro:
                return instance.swirlHydroAnim;
            case ElementType.Pyro:
                return instance.swirlPyroAnim;
            case ElementType.Cryo:
                return instance.swirlCryoAnim;
            default:
                return null;
        }
    }
    
    public static GameObject GetCrystallizationShield(ElementType type)
    {
        switch (type)
        {
            case ElementType.Electro:
                return instance.crystallizationElectroShield;
            case ElementType.Hydro:
                return instance.crystallizationHydroShield;
            case ElementType.Pyro:
                return instance.crystallizationPyroShield;
            case ElementType.Cryo:
                return instance.crystallizationCryoShield;
            default:
                return null;
        }
    }
    
    
    public static Color GetElementDamageColor(ElementType type)
    {
        switch (type)
        {
            case ElementType.Anemo:
                return AnemoDamageColor;
            case ElementType.Geo:
                return GeoDamageColor;
            case ElementType.Electro:
                return ElectroDamageColor;
            case ElementType.Dendro:
                return DendroDamageColor;
            case ElementType.Hydro:
                return HydroDamageColor;
            case ElementType.Pyro:
                return PyroDamageColor;
            case ElementType.Cryo:
                return CryoDamageColor;
            default:
                return Color.white;
        }
    }

}
