using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Interpreter
{
    // 判断地块x是否为高台
    public static bool isHigh(TileType x)
    {
        switch (x)
        {
            case TileType.wall:
            case TileType.highGround:
            case TileType.box:
                return true;
            default:
                return false;
        }
    }
    
    // 判断地块x是否为地面
    public static bool isLow(TileType x)
    {
        switch (x)
        {
            case TileType.lowGround:
            case TileType.danGround:
            case TileType.hole:
                return true;
            default:
                return false;
        }
    }

    // 判断地块x是否可以放置干员
    public static bool canPut(TileType x, bool banLowGround = false, bool banHighGround = false)
    {
        switch (x)
        {
            case TileType.lowGround:
                if (banLowGround) return false;
                return true;
            case TileType.highGround:
                if (banHighGround) return false;
                return true;
            default:
                return false;
        }
    }
    
    // 判断地块x是否可以让敌人通过
    public static bool canPass(TileType x)
    {
        switch (x)
        {
            case TileType.lowGround:
            case TileType.danGround:
                return true;
            default:
                return false;
        }
    }
    
}
