using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementCore : PropertyCore
{
    public List<ElementTimer> elementTimerList = new List<ElementTimer>();
    protected ElementTimer defaultElementTimer;
    
    // 附着元素集合
    private Dictionary<ElementType,float> attachedElement = new Dictionary<ElementType,float>();
    private float eleDecreaseSpeed = 0.3f;                          // 每秒元素自然消失量
    
    // 元素数据
    public ValueBuffer elementMastery = new ValueBuffer(0);    
    public ValueBuffer elementDamage = new ValueBuffer(1); 
    public ValueBuffer elementResistance = new ValueBuffer(0);



    protected override void Start_Property_Down()
    {
        defaultElementTimer = new ElementTimer(this);
        Start_ElementCore_Down();
    }

    protected virtual void Start_ElementCore_Down() {}

    protected override void Update_Property_Down()
    {
        foreach (var timer in elementTimerList)
        {
            timer.Update();
        }
        DecreaseAttachedElement();

        Update_ElementCore_Down();
    }
    
    protected virtual void Update_ElementCore_Down() {}

    private void DecreaseAttachedElement()
    {// 元素随时间自然消失
        for (int i = 0; i < attachedElement.Count; i++)
        {
            var tmp = attachedElement.ElementAt(i);
            ElementType type = tmp.Key;
            if (tmp.Value <= 0)         // 如果该元素已完全消失，移除该元素
            {
                attachedElement.Remove(type);
                i--;
                continue;
            }
            attachedElement[type] -= Time.deltaTime;
        }
    }


    /// <summary>  
    /// 计算元素伤害加成下的伤害数值，并判断能否附着元素
    /// </summary>
    public (float, bool) CauseDamageElement(ElementCore tarElementCore, float damage,
        ElementSlot elementSlot, ElementTimer timer)
    {
        if (elementSlot.eleType == ElementType.Physics)
            return (damage, false);
        
        // 元素伤害加成
        damage *= elementDamage.val;
        return (damage, timer.AttachElement(tarElementCore));
    }

    /// <summary>  
    /// 受到一次元素伤害，并处理附着元素
    /// </summary>
    public void GetDamageElement(ElementCore attacker, float damage, DamageMode mode,
        ElementSlot elementSlot, bool attached)
    {
        if (elementSlot.eleType == ElementType.Physics)
        {
            GetDamageProperty(damage, mode);
            return;
        }

        if (attached)       // 受到元素附着，将发生反应
        {
            AttachedElement(attacker,elementSlot);
        }

        // 元素抗性
        damage *= (1 - elementResistance.val);
        GetDamageProperty(damage, mode);
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



    private void AttachedElement(ElementCore attacker, ElementSlot elementSlot)           
    {// 受到元素附着
        
        // 如果身上没有任何元素，直接附着即可
        if (attachedElement.Count == 0)
        {
            attachedElement.Add(elementSlot.eleType, elementSlot.eleCount);
            return;
        }
        
        // 如果身上有相同元素，说明该元素不会与已附着元素中的任一元素反应，直接取最大值即可
        if (attachedElement.ContainsKey(elementSlot.eleType))
        {
            attachedElement[elementSlot.eleType] =
                Mathf.Max(attachedElement[elementSlot.eleType], elementSlot.eleCount);
            return;
        }
        
        // 如果身上有其他元素，则遍历已附着的所有元素，分别判断能否发生反应，并触发这些反应
        

    }
}


public class ElementSlot
{
    public ElementType eleType;
    public float eleCount;

    public ElementSlot(ElementType ttype = ElementType.Physics, float count = 0)
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

public class ElementTimer
{
    private float maxDuring;        // 最大元素附着间隔
    private ElementCore elc_;
    
    private Dictionary<ElementCore, float> elementTimeDict;


    public ElementTimer(ElementCore elementCore, float maxDuringTime = 3)
    {
        elc_ = elementCore;
        maxDuring = maxDuringTime;
        elementTimeDict = new Dictionary<ElementCore, float>();

        elc_.elementTimerList.Add(this);
    }

    public void Update()
    {// 每帧更新，需要在ElementCore内注册调用

        for (int i = 0; i < elementTimeDict.Count; i++)
        {
            var tmp = elementTimeDict.ElementAt(i);
            ElementCore elementCore = tmp.Key;

            if (tmp.Value <= 0)         // 如果该ElementCore已完成冷却，则删除
            {
                elementTimeDict.Remove(elementCore);
                i--;
                continue;
            }

            elementTimeDict[elementCore] -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 判断目标能否被挂上元素，如果可以，让目标进入元素附着冷却
    /// </summary>
    public bool AttachElement(ElementCore elementCore)
    {
        if (elementTimeDict.ContainsKey(elementCore)) return false;
        elementTimeDict.Add(elementCore, maxDuring);
        return true;
    }
    
}