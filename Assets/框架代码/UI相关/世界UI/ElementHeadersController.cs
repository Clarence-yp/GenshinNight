using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHeadersController : MonoBehaviour
{
    public List<Image> elementImageList = new List<Image>();

    private ElementCore elc_;

    private void Awake()
    {
        elc_ = transform.parent.parent.GetComponent<ElementCore>();
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (var ele in elc_.attachedElement)
        {
            // 当身上附着冻元素或激元素时，不显示在头上
            if (ele.Key == ElementType.None || ele.Key == ElementType.Frozen || ele.Key == ElementType.Catalyze)
                continue;
            elementImageList[i].gameObject.SetActive(true);
            elementImageList[i].sprite = StoreHouse.GetElementSprite(ele.Key);
            i++;
        }

        for (; i < elementImageList.Count; i++)
        {
            elementImageList[i].gameObject.SetActive(false);
        }
        
    }
}
