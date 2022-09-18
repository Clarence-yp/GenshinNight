using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionTransfer : MonoBehaviour
{
    private void Start()
    {
        ReactionController.reactionTimer.Clear();
    }

    void Update()
    {
        ReactionController.reactionTimer.Update();
    }
}
