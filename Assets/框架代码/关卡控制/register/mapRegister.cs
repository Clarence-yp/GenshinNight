using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapRegister : MonoBehaviour
{
    
    [EnumLabel("地形类")]
    public TileType platform;

    public int len = 1;

    void Awake()
    {
        InitManager.Register(transform.position, new TileSlot(platform, len));
    }
}

public enum TileType : byte
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
    hole,
}

public class TileTypeSlot
{
    public TileType type;
    public int priority;

    public TileTypeSlot(TileType t, int p)
    {
        type = t;
        priority = p;
    }
}

public class TileSlot
{
    public TileType type;                   // 该方块当前的种类
    public List<TileTypeSlot> typeList;     // 方块种类列表，越往前越优先展现
    public int len;                             // 其他方块到该方块的距离
    public int maxPriority = 0;

    public TileSlot(TileType t, int ll)
    {
        type = t;
        typeList = new List<TileTypeSlot>();
        typeList.Add(new TileTypeSlot(type, maxPriority));
        len = ll;
    }

    public void AddType(TileTypeSlot typeSlot)
    {
        maxPriority = typeSlot.priority > maxPriority ? typeSlot.priority : maxPriority;
        typeList.Add(typeSlot);
        typeList.Sort((a, b) => -a.priority.CompareTo(b.priority));
        type = typeList[0].type;
    }

    public void DelType(TileTypeSlot typeSlot)
    {
        typeList.Remove(typeSlot);
        typeList.Sort((a, b) => -a.priority.CompareTo(b.priority));
        type = typeList[0].type;
    }
}

public class OperPut_TileType_Buff : BuffSlot
{
    private OperatorCore oc_;
    private TileSlot tile;
    private TileTypeSlot typeSlot;

    private bool end = false;

    public OperPut_TileType_Buff(OperatorCore operatorCore, TileSlot tile_)
    {
        oc_ = operatorCore;
        tile = tile_;
        if(Interpreter.isLow(tile_.type))
            typeSlot = new TileTypeSlot(TileType.danGround, tile.maxPriority + 100);
        else
            typeSlot = new TileTypeSlot(TileType.wall, tile.maxPriority + 100);
        
    }
    
    public override void BuffStart()
    {
        tile.AddType(typeSlot);
        oc_.DieAction += EndTrue;
    }

    public override void BuffEnd()
    {
        tile.DelType(typeSlot);
    }

    public override bool BuffEndCondition()
    {
        return end;
    }

    private void EndTrue(BattleCore bc_)
    {
        end = true;
    }

    public override void BuffUpdate() { }
}