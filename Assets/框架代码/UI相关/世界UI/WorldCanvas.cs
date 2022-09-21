using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : MonoBehaviour
{
    private void Awake()
    {
        OperUIManager.WorldCanvas = GetComponent<Canvas>();
    }
}
