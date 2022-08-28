using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCore : PropertyCore
{
    private List<ElementSlot> atcEleList = new List<ElementSlot>(); // 附着元素列表
    private float eleDecreaseSpeed = 0.3f;                          // 每秒元素自然消失量
    
    // 元素数据
    public ValueBuffer elementMastery = new ValueBuffer(0);    
    public ValueBuffer elementDamage = new ValueBuffer(1); 
    public ValueBuffer elementResistance = new ValueBuffer(0);



    protected override void Start_Property_Down()
    {
        Start_ElementCore_Down();
    }

    protected virtual void Start_ElementCore_Down() {}

    protected override void Update_Property_Down()
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
    /// 带元素的攻击输出
    /// </summary>
    public float CauseDamage(float mul, ElementSlot elementSlot)
    {
        float dam = CauseDamage(mul);
        
        // 元素伤害
        if (elementSlot.eleType != ElementType.Physics)
            dam *= elementDamage.val;

        return dam;
    }
    
    /// <summary>  
    /// 受到一次元素伤害
    /// </summary>
    public void GetDamage(float baseDamage, DamageMode mode, ElementSlot elementSlot)
    {
        ///////// 这里处理元素反应相关，后面再实现
        
        
        // 元素抗性
        if (elementSlot.eleType != ElementType.Physics) 
            baseDamage *= (1 - elementResistance.val / 100);
        GetDamage(baseDamage, mode);
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
        // return ElementReactionCalculate.Vaporize(firElement, sedElement, damage, mastery);
        return 0;//占位
    }

    private float Melt(ElementSlot firElement, ElementSlot sedElement,
        float damage, float mastery)                
    {
        // 融化反应
        // return ElementReactionCalculate.Melt(firElement, sedElement, damage, mastery);
        return 0;//占位
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

    public ElementSlot()
    {
        eleType = ElementType.Physics;
        eleCount = 0;
    }
    
    public ElementSlot(ElementType ttype)
    {
        eleType = ttype;
        eleCount = 0;
    }

    public ElementSlot(ElementType ttype, float count)
    {
        eleType = ttype;
        eleCount = count;
    }
}

public class ElementReactionCalculate
{
    // 专门用于计算元素反应伤害数值的类
    
    public static float Overloaded(int baseLevel, float elementAmount, float mastery)           
    {
        // 超载反应
        return mastery;
    }

    public static float Superconduct(int baseLevel, float elementAmount, float mastery)         
    {
        // 超导反应
        return mastery;
    }

    public static float ElectroCharged(int baseLevel, float elementAmount, float mastery)       
    {
        // 感电反应
        return mastery;
    }

    public static float Swirl(int baseLevel, float elementAmount, float mastery)                
    {
        // 扩散反应
        return mastery;
    }
    
    public static float Crystallization(int baseLevel, float elementAmount, float mastery)                
    {
        // 结晶反应
        return mastery;
    }

    public static float Vaporize(float damage, float mastery)            
    {
        // 蒸发反应
        return mastery;
    }

    public static float Melt(float damage, float mastery)                
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
    Cryo,       // 冰元素
    [EnumLabel("物理")]
    Physics     // 物理
}