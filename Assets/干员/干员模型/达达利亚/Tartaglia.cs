using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tartaglia : OperatorCore
{
    [Header("达达利亚的普攻箭")] 
    public GameObject norArrow;

    private BattleCore tarBattleCore;

    protected override void Start_OperatorCore_Down()
    {
        
    }

    public override void OnAttack()
    {
        var arrow = PoolManager.GetObj(norArrow);
        parabola par = arrow.GetComponent<parabola>();

        tarBattleCore = target;
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        par.Init(pos, tarBattleCore, 12f, norAttack);
    }

    private void norAttack()
    {
        Battle(this, tarBattleCore, 1, DamageMode.Physical);
    }
    
    
    
}
