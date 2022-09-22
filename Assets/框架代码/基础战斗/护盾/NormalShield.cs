using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalShield : MonoBehaviour
{
    private LifeController life_ = new LifeController();
    private ElementType elementType;
    private BattleCore bc_;

    public void Init(BattleCore battleCore, float maxLife, ElementType type)
    {
        for (int i = 0; i < battleCore.shieldList.Count; i++)
        {// 如果目标身上已经带有类型相同的护盾，则更新旧护盾的数值，移除新护盾
            NormalShield shield = battleCore.shieldList[i];
            if (shield.name == name)
            {
                shield.life_.InitBaseLife(maxLife);
                shield.elementType = type;
                PoolManager.RecycleObj(gameObject);
                return;
            }
        }
        
        life_.InitBaseLife(maxLife);
        elementType = type;
        
        bc_ = battleCore;
        bc_.shieldList.Add(this);
        bc_.DieAction += Die;

        transform.SetParent(battleCore.transform);
        Vector3 pos = new Vector3(0,0,0.4f);
        transform.localPosition = pos;
    }

    public float GetDamage(float damage, ElementType type)
    {
        if (elementType == ElementType.Geo)
            damage /= 1.5f;
        else if (elementType == type)
            damage /= 2.5f;

        damage /= (1 + bc_.shieldStrength.val);
        
        float ret = damage - life_.life <= 0 ? 0 : damage - life_.life;
        life_.GetDamage(damage);
        if (life_.life <= 0)
        {
            bc_.DieAction -= Die;
            bc_.shieldList.Remove(this);
            PoolManager.RecycleObj(gameObject);
        }
        return ret;
    }

    private void Die(BattleCore bbc_)
    {
        bc_.shieldList.Remove(this);
        PoolManager.RecycleObj(gameObject);
    }
    
}
