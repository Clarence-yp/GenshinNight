using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeSliderController : MonoBehaviour
{
    private BattleCore bc_;
    private const float graySpeed = 4;
    private const float TOLERANCE = 1e-3f;

    public Slider grayLifeSlider;
    public Slider blueLifeSlider;
    public Slider skillSlider;
    

    private void Awake()
    {
        bc_ = transform.parent.GetComponent<BattleCore>();
    }

    void Update()
    {
        Refresh();
        
        if (Math.Abs(blueLifeSlider.value - grayLifeSlider.value) < TOLERANCE) return;
        grayLifeSlider.value = Mathf.Lerp(grayLifeSlider.value, blueLifeSlider.value, graySpeed * Time.deltaTime);
    }

    void Refresh()
    {
        blueLifeSlider.value = bc_.life_.life / bc_.life_.val;
        skillSlider.value = bc_.sp_.sp / bc_.sp_.maxSp;
        grayLifeSlider.value = Math.Max(grayLifeSlider.value, blueLifeSlider.value);
    }
    
}
