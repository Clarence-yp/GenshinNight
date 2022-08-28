using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New operData",menuName = "myScript/operData")]
public class operData : ScriptableObject
{
    
    
    [EnumLabel("干员数据类型")]
    public operDataType type;
    [EnumLabel("技能类型")]
    public SkillNum skillType;


    [Header("干员名称")]
    public string Name;
    public string EnName;//干员英文名称

    [Header("元素")]
    [EnumLabel("elementType")] 
    public ElementType elementType;

    [Header("是否是3D素材")]
    public bool is3D;
    
    [Header("是否是医疗干员")]
    public bool isMedical;
    
    [Header("是否可以打无人机")]
    public bool canAtkDrone;
    
    [Header("是否禁止布置在地面或高台上")]
    public bool banLowGround;
    public bool banHighGround;
    
    [Header("干员各种立绘和图片")]
    public Sprite imageInQueue;
    public GameObject spineDefault;
    public Sprite operUIImage1;
    public Sprite operUIImage2;

    [Header("干员预制体")]
    public GameObject operPrefab;

    [Header("干员报到")] 
    public AudioClip Report;
    [Header("升级")] 
    public AudioClip LevelUP;
    [Header("精英化")] 
    public AudioClip[] ElitismUP = new AudioClip[2];
    [Header("加入队伍")]
    public List<AudioClip> Join = new List<AudioClip>();
    [Header("行动出发")]
    public AudioClip Leave;
    [Header("行动开始")]
    public AudioClip Begin;
    [Header("选中干员")]
    public List<AudioClip> Selected = new List<AudioClip>();
    [Header("部署")]
    public List<AudioClip> Deploy = new List<AudioClip>();
    [Header("作战中")]
    public List<AudioClip> Fighting = new List<AudioClip>();
    [Header("4星结束行动")]
    public AudioClip End_4Star;
    [Header("3星结束行动")]
    public AudioClip End_3Star;
    [Header("非3星结束行动")]
    public AudioClip End_Not3Star;
    [Header("行动失败")]
    public AudioClip End_Fail;
    [Header("信赖触摸")]
    public AudioClip Touch;
    
    [Header("属性")]
    public float atk;//攻击
    public float def;//防御
    public float magicDef;//法抗
    public float life;//生命
    public int maxBlock;//最大阻挡数
    public float maxAtkInterval;// 最小攻击间隔
    
    [Header("放置与撤退")]
    public int cost;//放置费用
    public int consumPlace;//消耗可放置人数
    public float reTime;//再部署时间
    
    [Header("各精英阶段的最大等级")]
    public int[] maxLevel = new int[3];//各精英阶段的最大等级

    [Header("元素属性")] 
    public float elementalMastery;
    public float elementalDamage;
    public float elementalResistance;
    public float spRecharge;
    
    
    [Header("干员特性与详细天赋")]
    [TextArea] public string[] Description = new string[3];
    [TextArea] public string[] talent1 = new string[3];
    [TextArea] public string[] talent2 = new string[3];
    
    [Header("等级成长数据")] 
    public float[] growingAtk = new float[3];//成长攻击
    public float[] growingDef = new float[3];//成长防御
    public float[] growingLife = new float[3];//成长生命
    [Header("精英化成长数据")] 
    public float[] elitismAtk = new float[2];//精英化攻击
    public float[] elitismDef = new float[2];//精英化防御
    public float[] elitismLife = new float[2];//精英化生命
    public float[] elitismMagicDef = new float[2];//精英化法抗
    public int[] elitismBlock = new int[2];//精英化阻挡
    public int[] elitismExp = new int[2];
    public int[] elitismCost = new int[2];
    [Header("信赖成长数据")] 
    public float trustAtk;//信赖攻击
    public float trustDef;//信赖防御
    public float trustLife;//信赖生命
    public float trustMagicDef;//信赖法抗
    [Header("潜能成长数据")] 
    public float[] potentialAtk = new float[6];//精英化攻击
    public float[] potentialDef = new float[6];//精英化防御
    public float[] potentialLife = new float[6];//精英化生命
    public float[] potentialMagicDef = new float[6];//精英化法抗
    public int[] potentialCost = new int[6];
    public float[] potentialReTime = new float[6];
    
    [Header("攻击范围表")] 
    public GameObject[] atkRange = new GameObject[3];//攻击范围

    [Header("攻击范围图标")] 
    public Sprite[] atkRangeImage = new Sprite[3];

    [Header("图鉴界面头像")]
    public Sprite illustratedBookImage;
    
    [EnumLabel("选择职业")]
    public ProfessionType profession;
    
    [EnumLabel("选择星级")]
    public starNum star;
    
    [Header("技能图标")]
    public Sprite[] skillImage = new Sprite[3];
    
    [Header("技能基础数据，详细数据在干员脚本中设置")]
    [Header("技能1")]
    public string skillName0;//技能名称
    [TextArea]
    public string[] description0 = new string[7];//技能描述
    public int[] initSP0 = new int[7];//初始sp
    public int[] maxSP0 = new int[7];//最大sp，可释放所需sp
    public float[] duration0 = new float[7];//技能持续时间
    public int[] expNeed0 = new int[6];//升到下一级需要的经验卡
    public int[] costNeed0 = new int[6];//升到下一级需要的龙门币
    [EnumLabel("回复类型")]
    public recoverType skill0_recoverType;
    [EnumLabel("释放类型")]
    public releaseType skill0_releaseType;
    
    [Header("技能基础数据，详细数据在干员脚本中设置")]
    [Header("技能2")]
    public string skillName1;//技能名称
    [TextArea]
    public string[] description1 = new string[7];//技能描述
    public int[] initSP1 = new int[7];//初始sp
    public int[] maxSP1 = new int[7];//最大sp，可释放所需sp
    public float[] duration1 = new float[7];//技能持续时间
    public int[] expNeed1 = new int[6];//升到下一级需要的经验卡
    public int[] costNeed1 = new int[6];//升到下一级需要的龙门币
    [EnumLabel("回复类型")]
    public recoverType skill1_recoverType;
    [EnumLabel("释放类型")]
    public releaseType skill1_releaseType;
    
    [Header("技能基础数据，详细数据在干员脚本中设置")]
    [Header("技能3")]
    public string skillName2;//技能名称
    [TextArea]
    public string[] description2 = new string[7];//技能描述
    public int[] initSP2 = new int[7];//初始sp
    public int[] maxSP2 = new int[7];//最大sp，可释放所需sp
    public float[] duration2 = new float[7];//技能持续时间
    public int[] expNeed2 = new int[6];//升到下一级需要的经验卡
    public int[] costNeed2 = new int[6];//升到下一级需要的龙门币
    [EnumLabel("回复类型")]
    public recoverType skill2_recoverType;
    [EnumLabel("释放类型")]
    public releaseType skill2_releaseType;
    
    
    /////////////////////非监视器操作区//////////////////
    public int elitism;
    public int level;
    public int trust;
    public int potential;
}

public enum operDataType : byte
{
    [EnumLabel("基本资料")]
    baseInfo,
    [EnumLabel("立绘图片")]
    image,
    [EnumLabel("语音集合")]
    audio,
    [EnumLabel("基础属性")]
    attribute,
    [EnumLabel("成长属性")]
    grow,
    [EnumLabel("技能数据")]
    skill
}

public enum SkillNum : byte
{
    [EnumLabel("技能1")]
    skill1,
    [EnumLabel("技能2")]
    skill2,
    [EnumLabel("技能3")]
    skill3
}

public enum recoverType : byte
{
    [EnumLabel("自动回复")]
    auto,
    [EnumLabel("攻击回复")]
    atk,
    [EnumLabel("受击回复")]
    beAtk
}

public enum releaseType : byte
{
    [EnumLabel("手动触发")]
    hand,
    [EnumLabel("自动触发")]
    auto,
    [EnumLabel("被动技能")]
    passive,
    [EnumLabel("攻击触发")]
    atk,
}

public enum starNum : byte
{
    [EnumLabel("1星")]
    star1,
    [EnumLabel("2星")]
    star2,
    [EnumLabel("3星")]
    star3,
    [EnumLabel("4星")]
    star4,
    [EnumLabel("5星")]
    star5,
    [EnumLabel("6星")]
    star6
}

public enum ProfessionType : byte
{
    [EnumLabel("先锋")]
    xianfeng,
    [EnumLabel("狙击")]
    juji,
    [EnumLabel("医疗")]
    yiliao,
    [EnumLabel("术士")]
    shushi,
    [EnumLabel("近卫")]
    jinwei,
    [EnumLabel("重装")]
    zhongzhuang,
    [EnumLabel("辅助")]
    fuzhu,
    [EnumLabel("特种")]
    tezhong
}