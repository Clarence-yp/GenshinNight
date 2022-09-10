using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        buffList.Clear();
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

    public override void BuffUpdate()
    {
        during -= Time.deltaTime;
    }

    public override bool BuffEndCondition()
    {
        return during <= 0;
    }
}