using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ElementCore : PropertyCore
{
    public List<ElementTimer> elementTimerList = new List<ElementTimer>();
    protected ElementTimer defaultElementTimer;
    public ReactionController reactionController;
    
    // 附着元素集合
    public Dictionary<ElementType, float> attachedElement = new Dictionary<ElementType, float>();
    // 附着元素自然消失
    private const float defaultDecreaseSpeed = 0.2f;       // 默认普通元素每秒衰减速率
    protected float frozen_Inc_DecSpeed = 0.1f;             // 冻元素每秒增加的元素衰减速率
    public Dictionary<ElementType, float> eleDecreaseSpeed = new Dictionary<ElementType, float>();

    // 元素数据
    public ValueBuffer elementMastery = new ValueBuffer(0);    
    public ValueBuffer elementDamage = new ValueBuffer(1); 
    public ValueBuffer elementResistance = new ValueBuffer(0);
    public ValueBuffer shieldStrength = new ValueBuffer(0);     // 0.1表示护盾量增加10%

    // 反应状态
    [HideInInspector] public bool superConducting;          // 当前是否处于超导状态下
    [HideInInspector] public bool electroCharging;          // 当前是否处于感电状态下
    [HideInInspector] public bool frozen;                   // 是否处于冻结状态

    protected override void Start_Core()
    {
        base.Start_Core();
        defaultElementTimer = new ElementTimer(this);
        reactionController = new ReactionController(this);
        InitDecreaseSpeed();
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        foreach (var timer in elementTimerList)
        {
            timer.Update();
        }
        DecreaseAttachedElement();
        reactionController.Update();
    }


    private void InitDecreaseSpeed()
    {
        eleDecreaseSpeed.Add(ElementType.Anemo, defaultDecreaseSpeed);
        eleDecreaseSpeed.Add(ElementType.Geo, defaultDecreaseSpeed);
        eleDecreaseSpeed.Add(ElementType.Electro, defaultDecreaseSpeed);
        eleDecreaseSpeed.Add(ElementType.Dendro, defaultDecreaseSpeed);
        eleDecreaseSpeed.Add(ElementType.Hydro, defaultDecreaseSpeed);
        eleDecreaseSpeed.Add(ElementType.Pyro, defaultDecreaseSpeed);
        eleDecreaseSpeed.Add(ElementType.Cryo, defaultDecreaseSpeed);
        
        eleDecreaseSpeed.Add(ElementType.None, 1e9f);       // 如果空元素不慎附着，将立刻消失
        eleDecreaseSpeed.Add(ElementType.Frozen, 0.7f);    // 冻元素的基础消耗量略大
        eleDecreaseSpeed.Add(ElementType.Catalyze, 1e9f);   // 激元素暂时不考虑
    }
    
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

            if (type == ElementType.Frozen)     // 如果冻元素存在，则冻元素衰减速率增加
                eleDecreaseSpeed[ElementType.Frozen] += Time.deltaTime * frozen_Inc_DecSpeed;
            
            attachedElement[type] -= Time.deltaTime * eleDecreaseSpeed[type];
        }
    }
    
    
    
    /// <summary>  
    /// 计算元素伤害加成下的伤害数值，并判断能否附着元素
    /// </summary>
    public bool CauseDamageElement(ElementCore tarElementCore, ref float damage,
        ElementSlot elementSlot, ElementTimer timer = null)
    {
        if (elementSlot.eleType == ElementType.None)
            return false;
        
        // 元素伤害加成
        damage *= elementDamage.val;
        return timer != null && timer.AttachElement(tarElementCore);
    }

    /// <summary>  
    /// 受到一次伤害，并处理附着元素
    /// isBig==true，则展示大伤害
    /// haveText==true，isBig==false，则展示小伤害
    /// haveText==false，isBig==false，不展示伤害
    /// swirl==true，表示为扩散伤害，反应精通为0
    /// </summary>
    public void GetDamage(ElementCore attacker, float damage, DamageMode mode, ElementSlot elementSlot,
        bool attached, bool haveText = false, bool isBig = false, bool swirl = false)
    {
        if (elementSlot.eleType != ElementType.None) 
        {
            if (attached)       // 受到元素附着，将发生反应
            {
                ElementSlot element2 = new ElementSlot(elementSlot.eleType, elementSlot.eleCount);
                AttachedElement(attacker, element2, ref damage, ref isBig, swirl);
            }

            // 元素抗性
            damage *= (1 - elementResistance.val);
        }
        
        damage = GetDamageProperty(damage, mode);   // 计算防御和法抗

        // 身上所有的护盾先吃一遍伤害，最终受到的伤害由护盾挡下伤害后剩余最少的那个决定
        float finalDamage = damage;
        for (int i = 0; i < shieldList.Count; i++)
        {
            var shield = shieldList[i];
            finalDamage = Mathf.Min(finalDamage, shield.GetDamage(damage, elementSlot.eleType));
            if (!shield.isActiveAndEnabled) i--;
        }
        
        life_.GetDamage(finalDamage);               // 最终受到的伤害

        // 显示伤害数字
        if (!haveText && !isBig) return;
        GameObject damageText;
        if (isBig) damageText = PoolManager.GetObj(StoreHouse.instance.bigDamageText);
        else damageText = PoolManager.GetObj(StoreHouse.instance.smallDamageText);
        Text text = damageText.GetComponent<Text>();
        damageText.transform.SetParent(OperUIManager.WorldCanvas.transform);

        text.text = finalDamage.ToString("f0");
        text.color = StoreHouse.GetElementDamageColor(elementSlot.eleType);
        Vector3 center = transform.position;
        text.transform.position = center;
    }

    public void GetDamage(float damage, DamageMode mode)
    {
        ElementSlot none = new ElementSlot();
        GetDamage(null, damage, mode, none, false);
    }


    private void AttachedElement(ElementCore attacker, ElementSlot element2,
        ref float damage, ref bool isBig, bool swirl)           
    {// 受到元素附着，返回值为经过元素反应后的伤害值（蒸发融化等）
        
        // 如果身上没有任何元素，且元素不是风岩，直接附着即可
        if (attachedElement.Count == 0)
        {
            if (element2.eleType != ElementType.Anemo && element2.eleType != ElementType.Geo)
                attachedElement.Add(element2.eleType, element2.eleCount);
            return;
        }
        
        // 如果身上有相同元素，说明该元素不会与已附着元素中的任一元素反应，直接取最大值即可
        if (attachedElement.ContainsKey(element2.eleType))
        {
            attachedElement[element2.eleType] =
                Mathf.Max(attachedElement[element2.eleType], element2.eleCount);
            return;
        }
        
        // 如果身上有其他元素，则遍历已附着的所有元素，分别判断能否发生反应，并触发这些反应
        for (int i = 0; i < attachedElement.Count; i++)
        {
            var tmp = attachedElement.ElementAt(i);
            ElementSlot element1 = new ElementSlot(tmp.Key, tmp.Value);

            // 进行元素反应
            if (swirl)
            {// 如果是扩散过来的元素，反应精通为0
                reactionController.Reaction(attacker, element1, element2, 
                    0, ref damage, ref isBig);
            }
            else
            {// 吃后手攻击者的精通
                reactionController.Reaction(attacker, element1, element2,
                    attacker.elementMastery.val, ref damage, ref isBig);
            }
            
            if (element1.eleCount <= 0)         // 如果附着元素已完全消失，移除该元素
            {
                attachedElement.Remove(element1.eleType);
                i--;
            }
            if (element2.eleCount <= 0)         // 如果反应元素被全部耗尽，则退出附着
            {
                return;
            }
        }
        
        // 全部反应后若仍有剩余，且剩余元素不是风岩，剩余的元素将被附着
        if (element2.eleType != ElementType.Anemo && element2.eleType != ElementType.Geo)
            attachedElement.Add(element2.eleType, element2.eleCount);
    }
    
    public virtual void FrozenBegin() { }
    
    public virtual void FrozenEnd() { }
}


public class ElementSlot
{
    public ElementType eleType;
    public float eleCount;

    public ElementSlot(ElementType ttype = ElementType.None, float count = 0)
    {
        eleType = ttype;
        eleCount = count;
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
    None,       // 表示没有元素
    
    [EnumLabel("冻元素")]
    Frozen,     // 冻结反应产生的特殊元素
    [EnumLabel("激元素")]
    Catalyze,   // 原激化反应产生的特殊元素
}

public class ElementTimer
{
    private float maxDuring;        // 最大元素附着间隔
    private ElementCore elc_;       // 可能为null，表示无源计时器，此时需要手动调用Update
    
    private Dictionary<ElementCore, float> elementTimeDict;


    public ElementTimer(ElementCore elementCore, float maxDuringTime = 3)
    {
        elc_ = elementCore;
        maxDuring = maxDuringTime;
        elementTimeDict = new Dictionary<ElementCore, float>();

        if (elc_ != null) elc_.elementTimerList.Add(this);
        
    }

    public void Update()
    {// 每帧更新，需要在ElementCore内注册调用

        for (int i = 0; i < elementTimeDict.Count; i++)
        {
            var tmp = elementTimeDict.ElementAt(i);
            ElementCore elementCore = tmp.Key;
            elementTimeDict[elementCore] -= Time.deltaTime;

            if (elementTimeDict[elementCore] <= 0)         // 如果该ElementCore已完成冷却，则删除
            {
                elementTimeDict.Remove(elementCore);
                i--;
            }

        }
    }

    /// <summary>
    /// 判断目标能否被挂上元素，如果可以，让目标进入元素附着冷却
    /// </summary>
    public bool AttachElement(ElementCore elementCore)
    {
        if (maxDuring < 0) return true;
        if (elementTimeDict.ContainsKey(elementCore)) return false;
        elementTimeDict.Add(elementCore, maxDuring);
        return true;
    }

    public void Clear()
    {
        elementTimeDict.Clear();
    }
    
}