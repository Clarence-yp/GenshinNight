using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PropertyCore : MonoBehaviour
{
    // 基础数据
    public ValueBuffer atk_ = new ValueBuffer(0);
    public ValueBuffer def_ = new ValueBuffer(0);
    public ValueBuffer magicDef_ = new ValueBuffer(0);
    public LifeController life_ = new LifeController();
    public SPController sp_ = new SPController();
    

    // 造成伤害增加buff，val值表示百分比增加幅度（不可小于-1）
    private ValueBuffer causeDamInc_ = new ValueBuffer(0);
    // 造成伤害改变委托列表，由其他类注册，前委托的输出会作为后委托的输入
    private List<Func<float, float>> causeDamFuncList = new List<Func<float, float>>();
    // 受到伤害增加buff，val值表示百分比增加幅度（不可小于-1）
    private ValueBuffer getDamInc_ = new ValueBuffer(0);
    // 受到伤害改变委托列表，由其他类注册，前委托的输出会作为后委托的输入
    private List<Func<float, float>> getDamFuncList = new List<Func<float, float>>();
    
    
    void Start()
    {
        Start_Property_Down();
    }
    
    protected virtual void Start_Property_Down(){}
    
    void Update()
    {
        
        Update_Property_Down();
    }
    
    protected virtual void Update_Property_Down(){}
    
    
    /// <summary>  
    /// 基础输出伤害
    /// </summary>
    public float CauseDamage(float mul = 1f)
    {
        float dam = atk_.val * mul;
        dam *= causeDamInc_.val < -1 ? 0 : 1 + causeDamInc_.val;
        foreach (var damFunc in causeDamFuncList)
            dam = damFunc(dam);
        return dam;
    }

    /// <summary>  
    /// 受到一次伤害，计算并改变life_的值
    /// </summary>
    public void GetDamage(float baseDamage, DamageMode mode = DamageMode.Physical)
    {
        float dam = baseDamage;
        dam *= getDamInc_.val < -1 ? 0 : 1 + getDamInc_.val;
        foreach (var damFunc in getDamFuncList)
            dam = damFunc(dam);

        if (mode == DamageMode.Physical)
            dam = dam - def_.val < 0 ? 0 : dam - def_.val;
        else if (mode == DamageMode.Magic)
            dam = dam * (1 - (magicDef_.val / 100));
        
        life_.GetDamage(dam);
    }
    
    
}

public enum DamageMode
{
    Physical,
    Magic
}

public enum ValueBuffMode
{
    Fixed,
    Percentage
}

public class ValueBuffInner
{
    public ValueBuffMode mode;
    public float v;
}

public class ValueBuffer
{
    // 给buff使用的数值缓冲区

    public float val { get; private set; }
    public float baseVal { get; private set; }
    
    // 两种模式，固定数值增加，百分比增加（加算），百分比基于基础数值
    private List<ValueBuffInner> valueBuffList = new List<ValueBuffInner>();

    public ValueBuffer()
    {
        val = baseVal = 0;
    }
    public ValueBuffer(float v)
    {
        val = baseVal = v;
    }

    private void RefreshValue()
    {
        val = baseVal;
        foreach (var buffSlot in valueBuffList)
        {
            if (buffSlot.mode == ValueBuffMode.Fixed)
                val += buffSlot.v;
            else if(buffSlot.mode == ValueBuffMode.Percentage)
                val += buffSlot.v * baseVal;
        }
    }
    
    /// <summary>  
    /// 更改基础数值
    /// </summary>
    public void ChangeBaseValue(float newBaseVal)
    {
        baseVal = newBaseVal;
        RefreshValue();
    }
    
    /// <summary>  
    /// 增加基础数值
    /// </summary>
    public void AddBaseValue(float addBaseVal)
    {
        ChangeBaseValue(baseVal + addBaseVal);
    }
    
    
    /// <summary>  
    /// 加入新的数值buff
    /// </summary>
    public void AddValueBuff(ValueBuffInner buff)
    {
        valueBuffList.Add(buff);
        RefreshValue();
    }
    
    /// <summary>  
    /// 移除已有的数值buff
    /// </summary>
    public void DelValueBuff(ValueBuffInner buff)
    {
        valueBuffList.Remove(buff);
        RefreshValue();
    }
}

public class LifeController : ValueBuffer
{
    // lifeController父类的val为最大生命值，子类的life为当前生命值
    public float life { get; private set; }

    public void InitBaseLife(float v)
    {
        ChangeBaseValue(v);
        life = v;
    }

    public void RecoverCompletely()
    {
        life = val;
    }

    public void GetDamage(float damage)
    {
        life = life - damage < 0 ? 0 : life - damage;
    }

    public void GetHeal(float heal)
    {
        life = life + heal > val ? val : life + heal;
    }
}

public class SPController
{
    public float sp;
    public float maxSp;
    public ValueBuffer spRecharge = new ValueBuffer(1);
    
    public bool during;         // 技能是否开启
    public float remainingTime; // 技能剩余持续时间
    public float maxTime;       // 技能最大持续时间

    
}

public class DurationValueBuff : DurationBuffSlot
{
    private ValueBuffer valueBuffer;
    private ValueBuffInner buffInner;
    public DurationValueBuff(ValueBuffer buffer, ValueBuffMode buffMode, float v)
    {
        valueBuffer = buffer;
        buffInner = new ValueBuffInner();
        buffInner.mode = buffMode;
        buffInner.v = v;
    }

    public override void BuffStart()
    {
        valueBuffer.AddValueBuff(buffInner);
    }

    public override void BuffEnd()
    {
        valueBuffer.DelValueBuff(buffInner);
    }
}