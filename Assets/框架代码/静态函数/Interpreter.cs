using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Interpreter
{
    // 判断地块x是否为高台
    public static bool isHigh(platformType x)
    {
        switch (x)
        {
            case platformType.wall:
            case platformType.highGround:
            case platformType.box:
                return true;
            default:
                return false;
        }
    }
    
    // 判断地块x是否为地面
    public static bool isLow(platformType x)
    {
        switch (x)
        {
            case platformType.lowGround:
            case platformType.danGround:
            case platformType.hole:
                return true;
            default:
                return false;
        }
    }

    // 判断地块x是否可以放置干员
    public static bool canPut(platformType x)
    {
        switch (x)
        {
            case platformType.lowGround:
            case platformType.highGround:
                return true;
            default:
                return false;
        }
    }
    
    // 判断地块x是否可以让敌人通过
    public static bool canPass(platformType x)
    {
        switch (x)
        {
            case platformType.lowGround:
            case platformType.danGround:
                return true;
            default:
                return false;
        }
    }
    
}
