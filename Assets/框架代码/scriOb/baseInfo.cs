using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New baseInfo",menuName = "myScript/baseInfo")]
public class baseInfo : ScriptableObject
{
    [Header("属性")]
    public float atk;//攻击
    public float dev;//防御
    public float magicDev;//法抗
    public float life;//生命

    [Header("等级")]
    public int level;//等级
    public int elitism;//精英化等级

}
