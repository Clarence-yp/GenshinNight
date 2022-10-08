using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class skillPutOn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        string atkRangeName = oc_.GetSkillAtkRangeName(oc_.skillNum);
        if (atkRangeName == "") return;

        OperUIManager.rightUIController.ShowAtkRange(
            oc_, SearchAndGive.atkRangePos[atkRangeName]);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        string atkRangeName = oc_.GetSkillAtkRangeName(oc_.skillNum);
        if (atkRangeName == "") return;
        
        OperUIManager.rightUIController.ShowAtkRange(oc_);
    }
}
