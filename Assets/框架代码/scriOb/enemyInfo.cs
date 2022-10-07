using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New enemyInfo",menuName = "myScript/enemyInfo")]
public class enemyInfo : ScriptableObject
{
    [EnumLabel("敌人数据类型")]
    public enemyInfoType type;
    
    
    [Header("属性")]
    public string Name;//敌人名称
    public float atk;//攻击
    public float def;//防御
    public float magicDef;//法抗
    public float life = 100;//生命
    public int consumeBlock = 1; //消耗阻挡数
    public float minAtkInterval = 1; //最小攻击间隔
    [TextArea]
    public string talentDescription;

    [Header("元素")]
    [EnumLabel("elementType")] 
    public ElementType elementType;
    
    [Header("元素属性")]
    public float elementalMastery;
    public float elementalDamage = 1;
    public float elementalResistance;
    public float spRecharge = 1;
    public float shieldStrength;
    
    [Header("瞄准模式")]
    [EnumLabel("aimingMode")] 
    public AimingMode aimingMode;

    [Header("死亡后掉落龙门币和经验卡数量")] 
    public int dropCost;
    public int dropExp;
    
    [Header("消耗关卡生命值")] 
    public int consumeHP = 1;
    
    [Header("初始速度")]
    public float speed = 1;

    [Header("重量")] 
    public int mass = 1;
    
    [Header("勾选后敌人会沿直线在路径点间移动")] 
    public bool isDrone;

    [Header("头像与预制体")] 
    public Sprite headImage;
    public GameObject enemyPrefab;
    
    [Header("技能1")] 
    public string skillName;
    [EnumLabel("回复类型")]
    public recoverType skill_recoverType;
    [EnumLabel("释放类型")]
    public releaseType skill_releaseType;
    public float initSP;
    public float maxSP;
    public float duration;
    [TextArea]
    public string skillDescription;
    
    
}

public enum enemyInfoType : byte
{
    [EnumLabel("基本资料")]
    baseInfo,
    [EnumLabel("基础属性")]
    attribute,
    [EnumLabel("技能数据")]
    skill
}