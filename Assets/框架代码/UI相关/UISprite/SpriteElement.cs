using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteElement : MonoBehaviour
{
    private static SpriteElement instance;
    
    [Header("OperUI相关")]
    public Sprite elitismSprite0_p;    
    public static Sprite elitismSprite0;
    public Sprite elitismSprite1_p;
    public static Sprite elitismSprite1;
    public Sprite elitismSprite2_p;
    public static Sprite elitismSprite2;
    public Sprite anemoSprite_p;
    public static Sprite anemoSprite;
    public Sprite geoSprite_p;
    public static Sprite geoSprite;
    public Sprite electroSprite_p;
    public static Sprite electroSprite;
    public Sprite dendroSprite_p;
    public static Sprite dendroSprite;
    public Sprite hydroSprite_p;
    public static Sprite hydroSprite;
    public Sprite pyroSprite_p;
    public static Sprite pyroSprite;
    public Sprite cryoSprite_p;
    public static Sprite cryoSprite;    
    
    
    [Header("干员放置相关")]
    public GameObject atkRangeImage_p;     // 干员的攻击范围展示条纹图片
    public static GameObject atkRangeImage;
    
    
    
    
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
        elitismSprite0 = elitismSprite0_p;
        elitismSprite1 = elitismSprite1_p;
        elitismSprite2 = elitismSprite2_p;
        anemoSprite = anemoSprite_p;
        geoSprite = geoSprite_p;
        electroSprite = electroSprite_p;
        dendroSprite = dendroSprite_p;
        hydroSprite = hydroSprite_p;
        pyroSprite = pyroSprite_p;
        cryoSprite = cryoSprite_p;


        // 干员放置相关
        atkRangeImage = atkRangeImage_p;
    }

    public static Sprite GetElementSprite(ElementType type)
    {
        switch (type)
        {
            case ElementType.Anemo:
                return anemoSprite;
            case ElementType.Geo:
                return geoSprite;
            case ElementType.Electro:
                return electroSprite;
            case ElementType.Dendro:
                return dendroSprite;
            case ElementType.Hydro:
                return hydroSprite;
            case ElementType.Pyro:
                return pyroSprite;
            case ElementType.Cryo:
                return cryoSprite;
            default:
                return geoSprite;
        }
    }
    
}
