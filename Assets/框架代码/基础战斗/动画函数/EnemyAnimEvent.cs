using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{
    private EnemyCore ec_;
    
    private void Awake()
    {
        ec_ = transform.parent.GetComponent<EnemyCore>();
    }
    
    
    public void OnAttack()
    {
        ec_.OnAttack();
    }

    public void OnDie()
    {
        ec_.OnDie();
    }
    
}
