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
    public Image skillSliderFill;

    private Color32 spSkillColor = new Color32(200, 255, 0, 160);
    private Color32 duringSkillColor = new Color32(248, 183, 8, 160);
    

    private void Awake()
    {
        bc_ = transform.parent.GetComponent<BattleCore>();
        bc_.frontCanvas = GetComponent<Canvas>();
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
        grayLifeSlider.value = Math.Max(grayLifeSlider.value, blueLifeSlider.value);
        
        SPController sp_ = bc_.sp_;
        if (sp_.during)
        {
            skillSliderFill.color = duringSkillColor;
            if (sp_.maxTime <= 1e-6) return;
            skillSlider.value = sp_.remainingTime / sp_.maxTime;
        }
        else
        {
            skillSliderFill.color = spSkillColor;
            skillSlider.value = sp_.sp / sp_.maxSp;
        }
        
        
        
    }
    
}
