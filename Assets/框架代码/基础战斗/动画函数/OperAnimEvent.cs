using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperAnimEvent : MonoBehaviour
{
    private OperatorCore oc_;

    private void Start()
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

    public void skill1()
    {
        oc_.Skill1();
    }
    
    public void skill2()
    {
        oc_.Skill2();
    }
    
    public void skill3()
    {
        oc_.Skill3();
    }
    
}
