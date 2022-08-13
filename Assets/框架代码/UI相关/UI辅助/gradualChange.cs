using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class gradualChange : MonoBehaviour
{
    private const float TOLERANCE = 1e-2f;
    private float alpha = 0.0f;
    public float alphaSpeed;
    
    private CanvasGroup cg;
    
    void Awake ()
    {        
        cg = transform.GetComponent<CanvasGroup>();
        alpha = cg.alpha;
    }
    
    void Update ()
    {
        if (alpha != cg.alpha)
        {
            cg.alpha = Mathf.Lerp(cg.alpha,alpha,alphaSpeed);
            if (Mathf.Abs(alpha-cg.alpha)<=0.01)
            {
                cg.alpha = alpha;
            }
        }
    }
    
    public void Show()
    {
        alpha = 1;
        cg.blocksRaycasts = true;//可以和该UI对象交互
    }
    
    public void Hide()
    {
        alpha = 0;
        cg.blocksRaycasts = false;//不可以和该UI对象交互
    }

    public void ImmediateShow()
    {
        alpha = cg.alpha = 1;
        cg.blocksRaycasts = true;//不可以和该UI对象交互
    }
    
    public void ImmediateHide()
    {
        alpha = cg.alpha = 0;
        cg.blocksRaycasts = false;//不可以和该UI对象交互
    }

    public bool isShow()
    {
        return Math.Abs(cg.alpha - 1) < TOLERANCE;
    }
    
    public bool isHide()
    {
        return Math.Abs(cg.alpha) < TOLERANCE;
    }

}
