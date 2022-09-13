using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kroos : OperatorCore
{
    protected override void Start_OperatorCore_Down()
    {
        
    }


    public override void SkillStart_1()
    {
        float during = od_.duration0[skillLevel[0]];
        DurationValueBuff valueBuff = new DurationValueBuff(atk_, ValueBuffMode.Percentage, 0.1f,
            during);
        BuffManager.AddBuff(valueBuff);
    }

    public override void SkillStart_2()
    {
        float during = od_.duration0[skillLevel[0]];
        DurationAtkSpeedBuff atkSpeedBuff = new DurationAtkSpeedBuff(atkSpeedController, 200,
            during);
        BuffManager.AddBuff(atkSpeedBuff);
    }
}
