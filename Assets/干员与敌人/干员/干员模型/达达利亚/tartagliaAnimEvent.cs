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

    public void Skill3_Begin()
    {
        tartaglia.Skill3_Begin();
    }

    public void Skill3_CauseDamage()
    {
        tartaglia.Skill3CauseDamage();
    }
    
    
}
