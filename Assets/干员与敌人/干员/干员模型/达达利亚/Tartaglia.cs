using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tartaglia : OperatorCore
{
    [Header("达达利亚的普攻箭")] 
    public GameObject norArrow;
    public GameObject norHitAnim;
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

    private void norAttack(float multi)
    {
        GameObject hitAnim = PoolManager.GetObj(norHitAnim);
        hitAnim.transform.parent = target.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, target, true);
        BuffManager.AddBuff(recycleObj);
        
        
        ElementSlot elementSlot = new ElementSlot(ElementType.Cryo, 4f);
        Battle(tarBattleCore, atk_.val, DamageMode.Physical, elementSlot, defaultElementTimer);
    }
    
    
    
}
