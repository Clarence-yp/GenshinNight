using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowMan : EnemyCore
{
    [Header("弩手的特效")] 
    public ElementType elementType;
    public GameObject norArrow;
    public GameObject norHitAnim;

    private bool canAttachElement = false;

    public override void OnAttack()
    {
        var arrow = PoolManager.GetObj(norArrow);
        TrackMove tm = arrow.GetComponent<TrackMove>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        tm.Init(pos, this, target, 12f, norAttack);

        canAttachElement = defaultElementTimer.AttachElement(target);
    }
    
    private void norAttack(float multi, BattleCore tarBC, TrackMove tm)
    {
        GameObject hitAnim = PoolManager.GetObj(norHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        ElementSlot elementSlot = new ElementSlot(elementType, 1f);
        Battle(tarBC, atk_.val, DamageMode.Physical, elementSlot, canAttachElement, true);
        canAttachElement = false;
    }
    
    
}
