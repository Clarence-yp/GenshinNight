using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCore : BattleCore
{
    private List<ElementSlot> atcEleList = new List<ElementSlot>(); // 附着元素列表
    private float eleDecreaseSpeed = 0.3f;                          // 每秒元素自然消失量
    
    protected override void Start_BattleCore_Down()
    {

        Start_ElementCore_Down();
    }

    protected virtual void Start_ElementCore_Down() {}
    
    protected override void Update_BattleCore_Down()
    {

        Update_ElementCore_Down();
    }
    
    protected virtual void Update_ElementCore_Down() {}

    private void FixedUpdate()
    {
        for (int i = 0; i < atcEleList.Count; i++)
        {
            atcEleList[i].eleCount -= eleDecreaseSpeed;
            if (atcEleList[i].eleCount <= 0)
            {
                atcEleList.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>  
    /// 处理受到的元素伤害
    /// </summary>
    /// <param name="element">本次伤害的元素包</param>
    /// <param name="damage">本次伤害的数值</param>
    /// <param name="canAttach">本次伤害是否可以附着元素</param>
    /// <returns>本次伤害经过元素反应后应该得到的值</returns>
    public float SolveElementDamage(ElementSlot element, float damage, bool canAttach)
    {
        float realDamage = damage;


        return realDamage;
    }

    private void Overloaded(ElementSlot firElement, ElementSlot sedElement, float mastery)           
    {
        // 超载反应
    }

    private void Superconduct(ElementSlot firElement, ElementSlot sedElement, float mastery)         
    {
        // 超导反应
    }

    private void ElectroCharged(ElementSlot firElement, ElementSlot sedElement, float mastery)       
    {
        // 感电反应
    }

    private void Swirl(ElementSlot firElement, ElementSlot sedElement, float mastery)                
    {
        // 扩散反应
    }
    
    private void Crystallization(ElementSlot firElement, ElementSlot sedElement, float mastery)                
    {
        // 结晶反应
    }

    private float Vaporize(ElementSlot firElement, ElementSlot sedElement,
        float damage, float mastery)            
    {
        // 蒸发反应
        return ElementReactionCalculate.Vaporize(firElement, sedElement, damage, mastery);
    }

    private float Melt(ElementSlot firElement, ElementSlot sedElement,
        float damage, float mastery)                
    {
        // 融化反应
        return ElementReactionCalculate.Melt(firElement, sedElement, damage, mastery);
    }



    private void AddElement()           
    {
        // 在附着元素列表里添加元素
    }

    private void RemoveElement()        
    {
        // 从附着元素列表里移除元素
    }
    
}



public class ElementSlot
{
    public ElementType eleType;
    public float eleCount;
}

class ElementReactionCalculate
{
    // 专门用于计算元素反应伤害数值的类
    
    public static float Overloaded(ElementSlot firElement, ElementSlot sedElement, float mastery)           
    {
        // 超载反应
        return mastery;
    }

    public static float Superconduct(ElementSlot firElement, ElementSlot sedElement, float mastery)         
    {
        // 超导反应
        return mastery;
    }

    public static float ElectroCharged(ElementSlot firElement, ElementSlot sedElement, float mastery)       
    {
        // 感电反应
        return mastery;
    }

    public static float Swirl(ElementSlot firElement, ElementSlot sedElement, float mastery)                
    {
        // 扩散反应
        return mastery;
    }
    
    public static float Crystallization(ElementSlot firElement, ElementSlot sedElement, float mastery)                
    {
        // 结晶反应
        return mastery;
    }

    public static float Vaporize(ElementSlot firElement, ElementSlot sedElement,
        float damage, float mastery)            
    {
        // 蒸发反应
        return mastery;
    }

    public static float Melt(ElementSlot firElement, ElementSlot sedElement,
        float damage, float mastery)                
    {
        // 融化反应
        return mastery;
    }
}

public enum ElementType : byte
{
    [EnumLabel("风元素")]
    Anemo,      // 风元素
    [EnumLabel("岩元素")]
    Geo,        // 岩元素
    [EnumLabel("雷元素")]
    Electro,    // 雷元素
    [EnumLabel("草元素")]
    Dendro,     // 草元素
    [EnumLabel("水元素")]
    Hydro,      // 水元素
    [EnumLabel("火元素")]
    Pyro,       // 火元素
    [EnumLabel("冰元素")]
    Cryo        // 冰元素
}