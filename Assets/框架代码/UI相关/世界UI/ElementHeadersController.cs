using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementHeadersController : MonoBehaviour
{
    public List<Image> elementImageList = new List<Image>();
    private List<Animator> anim = new List<Animator>();
    private List<ElementType> elemenTypeList = new List<ElementType>();
    private ElementCore elc_;

    private void Awake()
    {
        elc_ = transform.parent.parent.GetComponent<ElementCore>();
        foreach (var i in elementImageList)
        {
            anim.Add(i.GetComponent<Animator>());
            elemenTypeList.Add(ElementType.None);
        }
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

            if (elemenTypeList[i] != ele.Key)
            {
                elemenTypeList[i] = ele.Key;
                elementImageList[i].gameObject.SetActive(true);
                elementImageList[i].sprite = StoreHouse.GetElementSprite(ele.Key);
                anim[i].SetBool("reset", false);
            }

            if (ele.Value <= 1) anim[i].SetBool("flicker", true);
            else anim[i].SetBool("flicker", false);
            i++;
        }

        for (; i < elementImageList.Count; i++)
        {
            if (elemenTypeList[i] != ElementType.None)
            {
                elementImageList[i].gameObject.SetActive(false);
                elemenTypeList[i] = ElementType.None;
                anim[i].SetBool("reset", true);
            }
        }
        
    }
}
