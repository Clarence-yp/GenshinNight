using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperAnimEvent : MonoBehaviour
{
    private OperatorCore oc_;

    private void Awake()
    {
        oc_ = transform.parent.GetComponent<OperatorCore>();
    }

    public void OnStart()
    {
        oc_.OnStart();
    }

    public void OnAttack()
    {
        oc_.OnAttack();
    }

    public void OnDie()
    {
        oc_.OnDie();
    }

    public void skill1()
    {
        oc_.SkillAtk_1();
    }
    
    public void skill2()
    {
        oc_.SkillAtk_2();
    }
    
    public void skill3()
    {
        oc_.SkillAtk_3();
    }
    
}
