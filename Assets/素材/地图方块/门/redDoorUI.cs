using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class redDoorUI : MonoBehaviour
{
    public Button startButton;
    
    public Image sliderImage;
    public gradualChange gc_;
    public Text detailText;
    public GameObject rewardUI;
    public Text costText;
    public Text expText;
    // public GameObject Canvas1;
    // public GameObject Canvas2;
    
    private void Awake()
    {
        // im_ = GameObject.Find("initManager").GetComponent<initManager>();
        // im_.redDoorWaveList.Add(this);
        // im_.redDoorSliderImage.Add(sliderImage);
        InitManager.Register(this);
        gc_.ImmediateHide();
        //can.worldCamera = im_.orthoCamera;
    }
    
    public void ChangeRewardUI(int cost, int exp)
    {
        costText.text = "+" + cost;
        expText.text = "+" + exp;
    }

    // public void DisableUI()
    // {
    //     Canvas1.SetActive(false);
    //     Canvas2.SetActive(false);
    // }
    //
    // public void EnableUI()
    // {
    //     Canvas1.SetActive(true);
    //     Canvas2.SetActive(true);
    // }
    
}
