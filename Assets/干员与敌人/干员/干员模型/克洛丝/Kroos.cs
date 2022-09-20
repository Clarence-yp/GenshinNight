using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kroos : OperatorCore
{
    private float[] atkIncreasePercentage = {0.1f, 0.15f, 0.2f, 0.3f, 0.35f, 0.4f, 0.5f};

    protected override void Update_OperatorCore_Down()
    {
        
    }

    // [Header("技能3")] 
    // public float[] atkSpeedIncrease = new float[7];

    public override void SkillStart_1()
    {
        // int lel = skillLevel[0];
        // float during = od_.duration0[lel];
        // SkillValueBuff valueBuff = new SkillValueBuff(atk_, ValueBuffMode.Percentage,
        //     atkIncreasePercentage[lel], sp_);
        // BuffManager.AddBuff(valueBuff);

        // SkillAtkSpeedBuff speedBuff = new SkillAtkSpeedBuff(atkSpeedController, 200, sp_);
        // BuffManager.AddBuff(speedBuff);
        
        DurationDizzyBuff buff = new DurationDizzyBuff(this, 2);
        BuffManager.AddBuff(buff);
    }

    public override void SkillStart_2()
    {
        
    }

    public override void OnAttack()
    {
        ElementSlot elementSlot = new ElementSlot(ElementType.Electro, 2f);
        Battle(target, atk_.val, DamageMode.Physical, elementSlot, defaultElementTimer);
    }
    
    
}
