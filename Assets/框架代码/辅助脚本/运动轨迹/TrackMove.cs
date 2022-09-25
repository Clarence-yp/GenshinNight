using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackMove : MonoBehaviour
{
    private BattleCore tarBattleCore;
    private BattleCore attacker;
    private Transform tarTrans;
    private bool isNull;
    private float speed = 5f;
    private float multi;
    private Action<float, BattleCore> reachFunc;
    
    private const float min_distance = 0.1f;
    private float distance;
    private Vector3 tarPos;
    
    public void Init(Vector3 pos, BattleCore attacker_, BattleCore targetBattleCore, float speed_ = 5,
        Action<float, BattleCore> reach = null, float Multi = 1)
    {
        transform.position = pos;
        tarBattleCore = targetBattleCore;
        attacker = attacker_;
        tarTrans = tarBattleCore.transform;
        speed = speed_;
        multi = Multi;
        reachFunc = reach;

        tarPos = tarTrans.position;
        distance = Vector3.Distance(transform.position, tarPos);

        if (targetBattleCore.dying)
        {
            reachFunc = null;
            PoolManager.RecycleObj(gameObject);
        }
        else
        {
            isNull = false;
            tarBattleCore.DieAction += TarNull;
        }
    }
    
    void Update()
    {
        if (!isNull)
        {
            tarPos = tarTrans.position;
            // if (Vector3.Distance(tarPos, Vector3.zero) > 200) 
            //     isNull = true;
        }

        // 朝向目标, 以计算运动
        transform.LookAt(tarPos);
        // 当前距离目标点
        float currentDist = Vector2.Distance(BaseFunc.xz(transform.position), BaseFunc.xz(tarPos));
        // 很接近目标了, 准备结束循环
        if (currentDist < min_distance)
        {
            Arrive();
            return;
        }
        // 平移 (朝向Z轴移动)
        transform.Translate(Vector3.forward * Mathf.Min(speed * Time.deltaTime, currentDist));
    }

    private void Arrive()
    {
        if (!isNull)
        {
            if (attacker.gameObject.activeSelf) reachFunc?.Invoke(multi, tarBattleCore);
            tarBattleCore.DieAction -= TarNull;
        }
        reachFunc = null;
        PoolManager.RecycleObj(gameObject);
    }
    
    private void TarNull(BattleCore bc_)
    {
        isNull = true;
    }
    
    
    
}
