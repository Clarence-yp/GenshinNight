using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffManager : MonoBehaviour
{
    private static BuffManager instance;

    // 所有buff的集合
    public static List<BuffSlot> buffList = new List<BuffSlot>();
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        foreach (var buff in buffList)
        {
            buff.BuffUpdate();
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            BuffSlot buff = buffList[i];
            if (buff.BuffEndCondition())
            {
                buff.BuffEnd();
                buffList.RemoveAt(i);
                i--;
            }
        }
    }

    public static void Init()
    {
        // buffList.Clear();
    }


    public static void Clear()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            BuffSlot buff = buffList[i];
            buff.BuffEnd();
            buffList.RemoveAt(i);
            i--;
        }
    }

    public static void AddBuff(BuffSlot buff)
    {
        buff.BuffStart();
        buffList.Add(buff);
    }
    
}

public abstract class BuffSlot
{
    public abstract void BuffStart();
    public abstract void BuffUpdate();
    public abstract bool BuffEndCondition();
    public abstract void BuffEnd();
}

public abstract class DurationBuffSlot : BuffSlot
{
    protected float during;

    public DurationBuffSlot(float t)
    {
        during = t;
    }

    public override void BuffUpdate()
    {
        during -= Time.deltaTime;
    }

    public override bool BuffEndCondition()
    {
        return during <= 0;
    }
}

public abstract class BattleCoreDurationBuff : BuffSlot
{
    protected float during;
    protected BattleCore bc_;

    protected bool isDie;
    
    public BattleCoreDurationBuff(BattleCore battleCore, float t)
    {
        bc_ = battleCore;
        during = t;

        isDie = bc_.dying;
    }

    public override void BuffStart()
    {
        if (!isDie) bc_.DieAction += Die;
    }

    public override void BuffUpdate()
    {
        during -= Time.deltaTime;
    }

    public override bool BuffEndCondition()
    {
        return during <= 0 || isDie;
    }

    public override void BuffEnd()
    {
        if(!isDie) bc_.DieAction -= Die;
    }
    
    private void Die(BattleCore battleCore)
    {
        isDie = true;
    }
}

public abstract class SkillBuffSlot : BuffSlot
{
    protected SPController sp_;
    protected BattleCore bc_;
    protected bool isDie;

    public SkillBuffSlot(BattleCore bcc_)
    {
        bc_ = bcc_;
        sp_ = bcc_.sp_;
        if (bc_.dying || !bc_.gameObject.activeSelf) isDie = true;
        else isDie = false;
    }

    public override void BuffStart()
    {
        bc_.DieAction += Die;
    }

    public override bool BuffEndCondition()
    {
        return !sp_.during || isDie;
    }

    public override void BuffEnd()
    {
        bc_.DieAction -= Die;
    }


    private void Die(BattleCore tmp)
    {
        isDie = true;
    }
}