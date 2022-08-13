using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class redDoorButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject detail;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        detail.SetActive(true);
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        detail.SetActive(false);
    }
}
