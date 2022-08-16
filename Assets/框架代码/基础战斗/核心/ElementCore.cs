using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCore : BattleCore
{
    private List<ElementSlot> atcEleList = new List<ElementSlot>(); // 附着元素列表
    private float eleDecreaseSpeed = 0.3f;                          // 每秒元素自然消失量

    public ValueBuffer elementalMastery = new ValueBuffer(0);
    
    public FightCalculation fightCalculation { get; private set; } = new FightCalculation();
    
    protected override void Start_BattleCore_Down()
    {

        Start_ElementCore_Down();
    }

    protected virtual void Start_ElementCore_Down() {}
    
    protected override void Update_BattleCore_Down()
    {
        fightCalculation.Update();
        CheckDie();

        if (dieNow) // 测试用，后期删掉
            fightCalculation.GetDamage(1e9f, DamageMode.Magic);
        
        
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
    
    private void CheckDie()
    {
        if (fightCalculation.life_.life <= 0)
        {
            public_DieAction?.Invoke(this);
            dieAction?.Invoke();
        }
    }
    
    /// <summary>  
    /// 表示该Core是否可以进行普攻
    /// </summary>
    protected bool CanAtk()
    {
        bool k = true;
        k = k & fightCalculation.CanAtk();
        return k;
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


public class FightCalculation : BattleCalculation
{
    // 元素数据
    public ValueBuffer mastery = new ValueBuffer(0);
    public ValueBuffer elementDamage = new ValueBuffer(1);
    public ValueBuffer elementResistance = new ValueBuffer(0);
    public ValueBuffer spRecharge = new ValueBuffer(1);
    
    public SPController sp_ = new SPController();
    
    public override void Update()
    {
        base.Update();
    }

    /// <summary>  
    /// 计算该Core一次输出的基础伤害*倍率
    /// </summary>
    public float CauseDamage(float mul, ElementSlot elementSlot)
    {
        float dam = atk_.val * mul;
        
        // 元素伤害
        if (elementSlot.eleType != ElementType.Physics)
            dam *= elementDamage.val;

        return Bat_CauseDamage(dam);
    }

    public float CauseDamage(float mul)
    {
        return CauseDamage(mul, new ElementSlot());
    }

    public float CauseDamage()
    {
        return CauseDamage(1f, new ElementSlot());
    }

    
    /// <summary>  
    /// 该Core受到一次伤害
    /// </summary>
    public void GetDamage(float baseDamage, DamageMode mode, ElementSlot elementSlot)
    {
        ///////// 这里处理元素反应相关，后面再实现
        
        
        // 元素抗性
        baseDamage *= (1 - elementResistance.val / 100);
        Bat_GetDamage(baseDamage,mode);
    }

    public void GetDamage(float baseDamage, DamageMode mode)
    {
        GetDamage(baseDamage, mode, new ElementSlot());
    }


    /// <summary>  
    /// attacker对defender造成一次伤害，结束后更新彼此数值
    /// </summary>
    public void Fight(ElementCore attacker, ElementCore defender,
        float multi, DamageMode mode, ElementSlot elementSlot)
    {
        float dam = attacker.fightCalculation.CauseDamage(multi, elementSlot);
        defender.fightCalculation.GetDamage(dam, mode, elementSlot);
    }
    
    public void Fight(ElementCore attacker, ElementCore defender, float multi, DamageMode mode)
    {
        Fight(attacker, defender, multi, mode, new ElementSlot());
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