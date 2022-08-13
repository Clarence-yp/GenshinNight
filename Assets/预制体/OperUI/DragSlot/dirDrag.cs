using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dirDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    private const float maxActiveRange = 300f;          // 移动箭头能活动的最大范围
    private const float putBorder = 250f;               // 移动距离大于该边界，则成功放置
    
    public DragSlot workingDragSlot;

    public Button BackGroundButton;
    private Transform prt;
    private RectTransform rectTransform;

    private bool draging;
    
    // 给atkRange的旋转默认值
    private Quaternion faceRight;
    private Quaternion faceUP;
    private Quaternion faceLeft;
    private Quaternion faceDown;
    private FourDirection direction = FourDirection.None;
    
    
    void Start()
    {
        prt = transform.parent;
        rectTransform = GetComponent<RectTransform>();

        faceRight = Quaternion.Euler(0, 0, 0);
        faceDown = Quaternion.Euler(0, 90, 0);
        faceLeft = Quaternion.Euler(0, 180, 0);
        faceUP = Quaternion.Euler(0, -90, 0);
    }
    
    void Update()
    {
        if (!draging)
        {
            rectTransform.anchoredPosition =
                Vector2.Lerp(rectTransform.anchoredPosition, Vector2.zero, 0.2f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {

        Vector2 pos = eventData.position - (Vector2) prt.position;
        float x = pos.x;
        float y = pos.y;
        
        float k = y / x;
        if (x > 0 && y > 0 && 0.8 * x + y > maxActiveRange)
            pos.x = maxActiveRange / (k + 0.8f);
        
        if (x < 0 && y > 0 && -0.8 * x + y > maxActiveRange)
            pos.x = maxActiveRange / (k - 0.8f);
        
        if (x < 0 && y < 0 && -0.8 * x - y > maxActiveRange)
            pos.x = -maxActiveRange / (k + 0.8f);
        
        if (x > 0 && y < 0 && 0.8 * x - y > maxActiveRange)
            pos.x = -maxActiveRange / (k - 0.8f);
        
        
        if (y - x < 0 && y + x > 0)
        {
            workingDragSlot.AnimTurnRight();
            if (direction != FourDirection.Right)
            {
                direction = FourDirection.Right;
                workingDragSlot.operatorCore.atkRange.transform.rotation = faceRight;
                workingDragSlot.operatorCore.atkRangeShowController.HideAtkRange();
                workingDragSlot.operatorCore.atkRangeShowController.ShowAtkRange();
            }
        }
        if (y - x > 0 && y + x > 0)
        {
            if (direction != FourDirection.UP)
            {
                direction = FourDirection.UP;
                workingDragSlot.operatorCore.atkRange.transform.rotation = faceUP;
                workingDragSlot.operatorCore.atkRangeShowController.HideAtkRange();
                workingDragSlot.operatorCore.atkRangeShowController.ShowAtkRange();
            }
        }
        if (y - x > 0 && y + x < 0)
        {
            workingDragSlot.AnimTurnLeft();
            if (direction != FourDirection.Left)
            {
                direction = FourDirection.Left;
                workingDragSlot.operatorCore.atkRange.transform.rotation = faceLeft;
                workingDragSlot.operatorCore.atkRangeShowController.HideAtkRange();
                workingDragSlot.operatorCore.atkRangeShowController.ShowAtkRange();
            }
        }
        if (y - x < 0 && y + x < 0)
        {
            if (direction != FourDirection.Down)
            {
                direction = FourDirection.Down;
                workingDragSlot.operatorCore.atkRange.transform.rotation = faceDown;
                workingDragSlot.operatorCore.atkRangeShowController.HideAtkRange();
                workingDragSlot.operatorCore.atkRangeShowController.ShowAtkRange();
            }
        }
        
        pos.y = k * pos.x;
        rectTransform.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draging = false;
        workingDragSlot.operatorCore.atkRangeShowController.HideAtkRange();
        Vector2 pos = rectTransform.anchoredPosition;
        
        if (pos.x > 0 && pos.y > 0 && 0.8 * pos.x + pos.y > putBorder ||
            pos.x < 0 && pos.y > 0 && -0.8 * pos.x + pos.y > putBorder ||
            pos.x < 0 && pos.y < 0 && -0.8 * pos.x - pos.y > putBorder ||
            pos.x > 0 && pos.y < 0 && 0.8 * pos.x - pos.y > putBorder)
        {
            workingDragSlot.PutSuccessfully();
        }
        else
        {
            direction = FourDirection.None;
        }
    }
}

public enum FourDirection
{
    Right,
    UP,
    Left,
    Down,
    None
}