using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testController : MonoBehaviour
{
    public operData dadaliya;
    
    private void Awake()
    {
        InitManager.Register(dadaliya, 1);
        InitManager.Register(dadaliya, 1);
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
