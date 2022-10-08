using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tartagliaAnimEvent : MonoBehaviour
{
    private Tartaglia tartaglia;

    private void Awake()
    {
        tartaglia = transform.parent.GetComponent<Tartaglia>();
    }

    public void ClearAtkInterval()
    {
        tartaglia.norAtkInterval = 0;
    }

    public void Skill2_Begin()
    {
        tartaglia.Skill2_Begin();
    }

    public void Skill2_CauseDamage()
    {
        tartaglia.Skill2CauseDamage();
    }
    
    
}
