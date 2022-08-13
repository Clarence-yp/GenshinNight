using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New skillData",menuName = "myScript/skillData")]
public class skillData : ScriptableObject
{
    [Header("技能介绍")]
    public Sprite[] skillImage = new Sprite[3];
    public string[] skillDescription = new string[3];
    
    [Header("1技能数据")]
    public int[] initSP1 = new int[7];//初始sp
    public int[] maxSP1 = new int[7];//最大sp，可释放所需sp
    public float[] duration1 = new float[7];//技能持续时间
    
    [Header("2技能数据")]
    public int[] initSP2 = new int[7];
    public int[] maxSP2 = new int[7];
    public float[] duration2 = new float[7];
    
    [Header("3技能数据")]
    public int[] initSP3 = new int[7];
    public int[] maxSP3 = new int[7];
    public float[] duration3 = new float[7];

}
