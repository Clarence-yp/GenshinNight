using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 对象池
    private const int maxCount = 64;
    private static Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();

    public static void RecycleObj(GameObject obj)
    {
        var par = Camera.main;
        var localPos = obj.transform.localPosition;
        obj.transform.SetParent(par.transform);
        obj.transform.localPosition = localPos;
        obj.SetActive(false);
         
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
        // 池子中有
        GameObject result = null;
        if (pool.ContainsKey(perfab.name))
        {
            if (pool[perfab.name].Count > 0)
            {
                result = pool[perfab.name][0];
                result.SetActive(true);
                pool[perfab.name].Remove(result);
                return result;
            }
        }
        // 池子中缺少
        result = Instantiate(perfab);
        result.name = perfab.name;
        RecycleObj(result);
        GetObj(result);
        return result;
    }
    
}
