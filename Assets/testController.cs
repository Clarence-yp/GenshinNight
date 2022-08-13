using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testController : MonoBehaviour
{
    public operData dadaliya;
    
    private void Awake()
    {
        InitManager.allOperDataList.Add(dadaliya);
        InitManager.allOperNumList.Add(1);
        InitManager.allOperDataList.Add(dadaliya);
        InitManager.allOperNumList.Add(1);
    }

    void Start()
    {
        InitManager.Init();
        OperUIManager.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
