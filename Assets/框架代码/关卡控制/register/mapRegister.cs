using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapRegister : MonoBehaviour
{
    
    [EnumLabel("地形类")]
    public platformType platform;

    public int len = 1;

    void Awake()
    {
        InitManager.Register(transform.position, new TileSlot(platform, len));
    }
}

public enum platformType : byte
{
    [EnumLabel("低处无法放置")]
    danGround,
    [EnumLabel("低处可以放置")]
    lowGround,
    [EnumLabel("高处无法放置")]
    wall,
    [EnumLabel("高处可以放置")]
    highGround,
    [EnumLabel("箱子类地形")]
    box,
    [EnumLabel("落穴")]
    hole
}

public class TileSlot
{
    public platformType type;       // 该方块的种类
    public int len;                 // 其他方块到该方块的距离

    public TileSlot(platformType t, int ll)
    {
        type = t;
        len = ll;
    }
}