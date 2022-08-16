using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAndGive : MonoBehaviour
{
    // 全局映射，存不同atkRange的显示偏移量
    public static Dictionary<string, List<Vector2>> atkRangePos = new Dictionary<string, List<Vector2>>();
    
    
    public List<Vector2> RangePos = new List<Vector2>();

    private BattleCore _bc;
    
    
    void Awake()
    {
        if (transform.parent != null) 
            _bc = transform.parent.GetComponent<BattleCore>();

        string str = name;
        str = str.Substring(0, str.Length - 7);
        if (atkRangePos.ContainsKey(str)) return;
        atkRangePos.Add(str, RangePos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("operator"))
        {
            ElementCore elementCore = other.GetComponent<ElementCore>();
            _bc.operatorList.Add(elementCore);
            elementCore.public_DieAction += DelBattleCore_Oper;
        }
        else if (other.CompareTag("enemy"))
        {
            ElementCore elementCore = other.GetComponent<ElementCore>();
            _bc.enemyList.Add(elementCore);
            elementCore.public_DieAction += DelBattleCore_Enemy;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pretendExit(other.gameObject);
    }
    
    
    /// <summary>
    /// 调用后，将模拟other物体离开该范围的事件
    /// </summary>
    public void pretendExit(GameObject other)
    {
        if (other.CompareTag("operator"))
        {
            ElementCore elementCore = other.GetComponent<ElementCore>();
            _bc.operatorList.Remove(elementCore);
            elementCore.public_DieAction -= DelBattleCore_Oper;
        }
        else if (other.CompareTag("enemy"))
        {
            ElementCore elementCore = other.GetComponent<ElementCore>();
            _bc.enemyList.Remove(elementCore);
            elementCore.public_DieAction -= DelBattleCore_Enemy;
        }
    }


    private void DelBattleCore_Oper(ElementCore dying_bc)
    {
        // 用于死亡的回调函数，在List里删掉要死的这个ElementCore
        _bc.operatorList.Remove(dying_bc);
    }
    
    private void DelBattleCore_Enemy(ElementCore dying_bc)
    {
        _bc.enemyList.Remove(dying_bc);
    }
    
    
    
}
