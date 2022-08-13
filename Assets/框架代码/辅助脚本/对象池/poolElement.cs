
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poolElement : MonoBehaviour
{
    private static poolElement instance;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        
        Init();
    }

    private void Init()
    {
        
    }
}
