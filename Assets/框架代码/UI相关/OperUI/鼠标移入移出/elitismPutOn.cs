using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class elitismPutOn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        if (oc_.eliteLevel >= 2) return;
        OperUIManager.levelUIController.elitismCostAnim.SetBool("out", true);
        OperUIManager.levelUIController.talentText.text = oc_.od_.Description[oc_.eliteLevel + 1];
        OperUIManager.rightUIController.ShowAtkRange(oc_,
            SearchAndGive.atkRangePos[oc_.od_.atkRange[oc_.eliteLevel + 1].name]);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        if (oc_.eliteLevel >= 2) return;
        OperUIManager.levelUIController.elitismCostAnim.SetBool("out", false);
        OperUIManager.levelUIController.talentText.text = oc_.od_.Description[oc_.eliteLevel];
        OperUIManager.rightUIController.ShowAtkRange(oc_);
    }
}
