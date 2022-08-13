using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New enemyInfo",menuName = "myScript/enemyInfo")]
public class enemyInfo : ScriptableObject
{
    
    [Header("属性")]
    public string Name;//敌人名称
    public float atk;//攻击
    public float def;//防御
    public float magicDef;//法抗
    public float life;//生命
    public int consumeBlock; //消耗阻挡数
    public float minAtkInterval; //最小攻击间隔
    [TextArea]
    public string talentDescription;

    [Header("死亡后掉落龙门币和经验卡数量")] 
    public int dropCost;
    public int dropExp;
    
    [Header("初始速度")]
    public float speed;
    
    [Header("恐惧抗性")]
    public int fearResistance;
    
    [Header("勾选后敌人会沿直线在路径点间移动")] 
    public bool isDrone;

    [Header("头像与预制体")] 
    public Sprite headImage;
    public GameObject enemyPrefab;
    
    [Header("技能1")] 
    public string skillName;
    public releaseType ReleaseType;
    public float startSP;
    public float maxSP;
    public float during;
    [TextArea]
    public string skillDescription;
    
    [EnumLabel("回复类型")]
    public recoverType skill_recoverType;
}
