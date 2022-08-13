using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blueDoorRegister : MonoBehaviour
{
    private Spfa spfa_ = new Spfa();

    private void Awake()
    {
        InitManager.Register(spfa_);
        spfa_.RunSpfa(BaseFunc.FixCoordinate(transform.position));
    }
}
