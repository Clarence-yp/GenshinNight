using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroySelf : MonoBehaviour
{
    public void DESTROY()
    {
        Destroy(gameObject);
    }

    public void ActiveFlase()
    {
        gameObject.SetActive(false);
    }
    
}
