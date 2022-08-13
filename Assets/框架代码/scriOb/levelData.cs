using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New levelData",menuName = "myScript/levelData")]
public class levelData : ScriptableObject
{
    public int ID_set;
    public string ID_show;
    public string Name;
    public string recommendLevel;
    
    [TextArea] 
    public string Description;
    
    [TextArea] 
    public string HighLightDescription;

    [Header("地图快照")] 
    public Sprite map;

    [Header("背景音乐")] 
    public AudioClip BGM;
    
}
