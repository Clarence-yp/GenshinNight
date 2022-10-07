using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 对象池
    private const int maxCount = 64;
    private static Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    private static Dictionary<string, GameObject> objPrt = new Dictionary<string, GameObject>();
    
    public static void RecycleObj(GameObject obj)
    {
        if (obj == null) return;
        
        obj.SetActive(false);
        obj.transform.SetParent(objPrt[obj.name].transform);

        if (pool.ContainsKey(obj.name))
        {
            if (pool[obj.name].Count < maxCount)
            {
                pool[obj.name].Add(obj);
            }
        }
        else
        {
            pool.Add(obj.name, new List<GameObject>() { obj });
        }
    }
    
    public static GameObject GetObj(GameObject perfab)
    {
        // 如果没有父物体则生成
        if (!objPrt.ContainsKey(perfab.name))
        {
            objPrt.Add(perfab.name, new GameObject(perfab.name + "对象池"));
        }

        // 池子中有
        GameObject result = null;
        if (pool.ContainsKey(perfab.name))
        {
            if (pool[perfab.name].Count > 0)
            {
                result = pool[perfab.name][0];
                result.SetActive(true);
                pool[perfab.name].Remove(result);
                result.transform.SetParent(objPrt[perfab.name].transform);
                return result;
            }
        }
        // 池子中缺少
        result = Instantiate(perfab, null);
        result.name = perfab.name;
        RecycleObj(result);
        GetObj(result);
        result.transform.SetParent(objPrt[perfab.name].transform);
        return result;
    }
    
}

public abstract class PrtRecycleObj : BuffSlot
{
    protected BattleCore bc_;
    protected GameObject obj;
    protected bool isDie;
    
    public PrtRecycleObj(GameObject object_,BattleCore Prt)
    {
        obj = object_;
        bc_ = Prt;

        if (bc_.dying)
        {
            Die(bc_);
        }
        else bc_.DieAction += Die;
    }

    public override bool BuffEndCondition()
    {
        return isDie;
    }
    
    public override void BuffEnd()
    {
        if (!isDie)
        {
            bc_.DieAction -= Die;
        }
        PoolManager.RecycleObj(obj);
    }

    private void Die(BattleCore battleCore)
    {
        isDie = true;
    }
}

public class SkillRecycleObj : PrtRecycleObj
{
    private SPController sp_;

    public SkillRecycleObj(GameObject object_, BattleCore battleCore) : base(object_, battleCore)
    {
        sp_ = bc_.sp_;
    }

    public override void BuffStart() { }

    public override void BuffUpdate() { }

    public override bool BuffEndCondition()
    {
        return !sp_.during || base.BuffEndCondition();
    }
}