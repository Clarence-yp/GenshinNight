using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hound : EnemyCore
{
    protected override void Start_EnemyCore_Down()
    {
        // ElementType type = ElementType.Pyro;
        // GameObject shield = PoolManager.GetObj(StoreHouse.GetCrystallizationShield(type));
        // NormalShield normalShield = shield.GetComponent<NormalShield>();
        // normalShield.Init(this, 500, type);

    }

    protected override void Update_EnemyCore_Down()
    {
        // ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 8f);
        // Battle(this, 0, DamageMode.Physical, elementSlot, defaultElementTimer);
    }
    
    public override void OnAttack()
    {
        ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 2f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, defaultElementTimer);
    }
    
}
