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
        obj.SetActive(false);
        obj.transform.parent = objPrt[obj.name].transform;
        
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
                result.transform.parent = objPrt[perfab.name].transform;
                return result;
            }
        }
        // 池子中缺少
        result = Instantiate(perfab, null);
        result.name = perfab.name;
        RecycleObj(result);
        GetObj(result);
        result.transform.parent = objPrt[perfab.name].transform;
        return result;
    }
    
}
