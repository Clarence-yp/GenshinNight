using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class testController : MonoBehaviour
{
    public operData dadaliya;
    public operData kroos;
    public operData steward;
    public operData beagle;
    public operData melantha;
    public operData orchid;
    public operData catapult;

    private void Awake()
    {
        InitManager.Register(dadaliya, 1);
        InitManager.Register(kroos, 1);
        InitManager.Register(steward, 1);
        InitManager.Register(beagle, 1);
        InitManager.Register(melantha, 1);
        InitManager.Register(orchid, 1);
        InitManager.Register(catapult, 1);
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
