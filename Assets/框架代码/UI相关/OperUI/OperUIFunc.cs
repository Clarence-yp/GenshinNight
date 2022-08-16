using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OperUIFunc : MonoBehaviour
{

    /// <summary>
    /// 右侧UI的干员撤退按钮
    /// </summary>
    public void Exit()
    {
        
    }

    /// <summary>
    /// 右侧UI的技能释放按钮
    /// </summary>
    public void Skill()
    {
        
    }

    /// <summary>
    /// 干员精英化按钮
    /// </summary>
    public void Elitism()
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        BattleCalculation bc_ = oc_.fightCalculation;

        if (oc_.eliteLevel >= 2) return;
        if (oc_.level < od_.maxLevel[oc_.eliteLevel]) return;
        if (InitManager.resourceController.cost < od_.elitismCost[oc_.eliteLevel]) return;
        if (InitManager.resourceController.exp < od_.elitismExp[oc_.eliteLevel]) return;
        InitManager.resourceController.CostIncrease(-od_.elitismCost[oc_.eliteLevel]);
        InitManager.resourceController.ExpIncrease(-od_.elitismExp[oc_.eliteLevel]);
        

        bc_.atk_.AddBaseValue(od_.elitismAtk[oc_.eliteLevel]);
        bc_.def_.AddBaseValue(od_.elitismDef[oc_.eliteLevel]);
        bc_.magicDef_.AddBaseValue(od_.elitismMagicDef[oc_.eliteLevel]);
        bc_.life_.AddBaseValue(od_.elitismLife[oc_.eliteLevel]);
        bc_.maxBlock += od_.elitismBlock[oc_.eliteLevel];
        
        
        oc_.eliteLevel++;
        oc_.ChangeAtkRange();
        
        OperUIManager.Refresh();
    }
    
    /// <summary>
    /// 干员升级按钮
    /// </summary>
    public void LevelUp(int upLevel)
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        BattleCalculation bc_ = oc_.fightCalculation;

        int dx = Math.Min(upLevel, oc_.od_.maxLevel[oc_.eliteLevel] - oc_.level);
        if (dx == 0 || InitManager.resourceController.cost < dx ||
            InitManager.resourceController.exp < dx) return;
        InitManager.resourceController.CostIncrease(-dx);
        InitManager.resourceController.ExpIncrease(-dx);

        oc_.level += dx;
        bc_.atk_.AddBaseValue(od_.growingAtk[oc_.eliteLevel] * dx);
        bc_.def_.AddBaseValue(od_.growingDef[oc_.eliteLevel] * dx);
        bc_.life_.AddBaseValue(od_.growingLife[oc_.eliteLevel] * dx);
        
        OperUIManager.Refresh();
    }

    /// <summary>
    /// 清空再部署时间按钮
    /// </summary>
    public void Immediately()
    {
        
    }

    /// <summary>
    /// skillUI选择skill1
    /// </summary>
    public void Choose_Skill1()
    {
        OperUIManager.skillUIController.skillUISta = SkillUISta.skill1;
        OperUIManager.skillUIController.Refresh();
    }
    
    /// <summary>
    /// skillUI选择skill2
    /// </summary>
    public void Choose_Skill2()
    {
        OperUIManager.skillUIController.skillUISta = SkillUISta.skill2;
        OperUIManager.skillUIController.Refresh();
    }
    
    /// <summary>
    /// skillUI选择skill
    /// </summary>
    public void Choose_Skill3()
    {
        OperUIManager.skillUIController.skillUISta = SkillUISta.skill3;
        OperUIManager.skillUIController.Refresh();
    }
    
    /// <summary>
    /// skillUI选择detailedValues
    /// </summary>
    public void Choose_DetailedValues()
    {
        OperUIManager.skillUIController.skillUISta = SkillUISta.detailedValue;
        OperUIManager.skillUIController.Refresh();
    }
    
    /// <summary>
    /// skillUI选择detailedTalent
    /// </summary>
    public void Choose_DetailedTalent()
    {
        OperUIManager.skillUIController.skillUISta = SkillUISta.detailedTalent;
        OperUIManager.skillUIController.Refresh();
    }
    
    
    
    
    

    /// <summary>
    /// 技能升级按钮
    /// </summary>
    public void SkillLevelUp(int skillID)
    {
        
    }
    
    
}
