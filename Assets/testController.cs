using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class testController : MonoBehaviour
{
    public operData dadaliya;
    public operData kroos;

    private void Awake()
    {
        InitManager.Register(dadaliya, 1);
        InitManager.Register(dadaliya, 1);
        InitManager.Register(kroos, 1);
    }

    void Start()
    {
        InitManager.Init();
        OperUIManager.Init();
        BuffManager.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
