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

    [Header("元素反应相关")] 
    public GameObject overLoadAnim;
    public GameObject superConductAnim;
    public GameObject superConductDuration;



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
    
}
