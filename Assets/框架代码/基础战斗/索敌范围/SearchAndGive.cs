using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAndGive : MonoBehaviour
{

    public List<Vector2> RangePos = new List<Vector2>();

    private BattleCore _bc;
    
    
    void Awake()
    {
        _bc = transform.parent.GetComponent<BattleCore>();
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("operator"))
        {
            BattleCore battleCore = other.GetComponent<BattleCore>();
            _bc.operatorList.Add(battleCore);
            battleCore.battleCore_DieAction += DelBattleCore_Oper;
        }
        else if (other.CompareTag("enemy"))
        {
            BattleCore battleCore = other.GetComponent<BattleCore>();
            _bc.enemyList.Add(battleCore);
            battleCore.battleCore_DieAction += DelBattleCore_Enemy;
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
            BattleCore battleCore = other.GetComponent<BattleCore>();
            _bc.operatorList.Remove(battleCore);
            battleCore.battleCore_DieAction -= DelBattleCore_Oper;
        }
        else if (other.CompareTag("enemy"))
        {
            BattleCore battleCore = other.GetComponent<BattleCore>();
            _bc.enemyList.Remove(battleCore);
            battleCore.battleCore_DieAction -= DelBattleCore_Enemy;
        }
    }


    private void DelBattleCore_Oper(BattleCore dying_bc)
    {
        // 用于死亡的回调函数，在List里删掉要死的这个BattleCore
        _bc.operatorList.Remove(dying_bc);
    }
    
    private void DelBattleCore_Enemy(BattleCore dying_bc)
    {
        _bc.enemyList.Remove(dying_bc);
    }
    
    
    
}
