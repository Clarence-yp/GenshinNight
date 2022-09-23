using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class OperUIFunc : MonoBehaviour
{

    /// <summary>
    /// 右侧UI的干员撤退按钮
    /// </summary>
    public void Exit()
    {
        OperUIManager.showingOper.Retreat();
    }

    /// <summary>
    /// 右侧UI的技能释放按钮
    /// </summary>
    public void Skill()
    {
        SPController sp_ = OperUIManager.showingOper.sp_;
        if (!sp_.CanReleaseSkill() || sp_.outType != releaseType.hand) return;
        
        int skillNum = OperUIManager.showingOper.skillNum;
        switch (skillNum)
        {
            case 0:
                OperUIManager.showingOper.SkillStart_1();
                break;
            case 1:
                OperUIManager.showingOper.SkillStart_2();
                break;
            case 2:
                OperUIManager.showingOper.SkillStart_3();
                break;
        }
        
        sp_.ReleaseSkill();
        OperUIManager.CloseOperUI();
    }

    /// <summary>
    /// 干员精英化按钮
    /// </summary>
    public void Elitism()
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;

        if (oc_.eliteLevel >= 2) return;
        if (oc_.level < od_.maxLevel[oc_.eliteLevel]) return;
        if (InitManager.resourceController.cost < od_.elitismCost[oc_.eliteLevel]) return;
        if (InitManager.resourceController.exp < od_.elitismExp[oc_.eliteLevel]) return;

        oc_.level = 0;
        InitManager.resourceController.CostIncrease(-od_.elitismCost[oc_.eliteLevel]);
        InitManager.resourceController.ExpIncrease(-od_.elitismExp[oc_.eliteLevel]);
        
        oc_.atk_.AddBaseValue(od_.elitismAtk[oc_.eliteLevel]);
        oc_.def_.AddBaseValue(od_.elitismDef[oc_.eliteLevel]);
        oc_.magicDef_.AddBaseValue(od_.elitismMagicDef[oc_.eliteLevel]);
        oc_.life_.AddBaseValue(od_.elitismLife[oc_.eliteLevel]);
        oc_.maxBlock.AddBaseValue(od_.elitismBlock[oc_.eliteLevel]);

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

        int dx = Math.Min(upLevel, oc_.od_.maxLevel[oc_.eliteLevel] - oc_.level);
        if (dx == 0 || InitManager.resourceController.cost < dx ||
            InitManager.resourceController.exp < dx) return;
        InitManager.resourceController.CostIncrease(-dx);
        InitManager.resourceController.ExpIncrease(-dx);

        oc_.level += dx;
        oc_.atk_.AddBaseValue(od_.growingAtk[oc_.eliteLevel] * dx);
        oc_.def_.AddBaseValue(od_.growingDef[oc_.eliteLevel] * dx);
        oc_.life_.AddBaseValue(od_.growingLife[oc_.eliteLevel] * dx);
        
        OperUIManager.Refresh();
    }

    /// <summary>
    /// 清空再部署时间按钮
    /// </summary>
    public void Immediately()
    {
        OperatorCore oc_ = OperUIManager.showingOper;
        float reTime = InitManager.operReTime[oc_.operID];
        if (InitManager.resourceController.cost < reTime)
        {
            return;
        }
        InitManager.resourceController.CostIncrease(-reTime);
        InitManager.operReTime[oc_.operID] = 1e-5f;
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
    /// 技能选择按钮，同事重置spController
    /// </summary>
    public void SkillChoose()
    {
        int skillID = OperUIManager.skillUIController.showSkillNum;
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        oc_.skillNum = skillID;
        
        // 根据选择的技能设置spController，站场上选择清空所有sp
        int lel = oc_.skillLevel[oc_.skillNum];
        switch (oc_.skillNum)
        {
            case 0:
                oc_.sp_.Init(oc_, 0, od_.maxSP0[lel], od_.duration0[lel],
                    od_.skill0_recoverType, od_.skill0_releaseType, od_.spRecharge);
                break;
            case 1:
                oc_.sp_.Init(oc_, 0, od_.maxSP1[lel], od_.duration1[lel],
                    od_.skill1_recoverType, od_.skill1_releaseType, od_.spRecharge);
                break;
            case 2:
                oc_.sp_.Init(oc_, 0, od_.maxSP2[lel], od_.duration2[lel],
                    od_.skill2_recoverType, od_.skill2_releaseType, od_.spRecharge);
                break;
        }
        
        OperUIManager.Refresh();
    }
    
    /// <summary>
    /// 技能升级按钮
    /// </summary>
    public void SkillLevelUp()
    {
        int skillID = OperUIManager.skillUIController.showSkillNum;
        OperatorCore oc_ = OperUIManager.showingOper;
        operData od_ = oc_.od_;
        int skillLevel = oc_.skillLevel[skillID];

        if (skillLevel >= 6) return;

        int coseNeed = skillID switch
        {
            0 => od_.costNeed0[skillLevel],
            1 => od_.costNeed1[skillLevel],
            2 => od_.costNeed2[skillLevel],
            _ => throw new ArgumentOutOfRangeException(nameof(skillID), skillID, null)
        };
        int expNeed = skillID switch
        {
            0 => od_.expNeed0[skillLevel],
            1 => od_.expNeed1[skillLevel],
            2 => od_.expNeed2[skillLevel],
            _ => throw new ArgumentOutOfRangeException(nameof(skillID), skillID, null)
        };

        if (InitManager.resourceController.cost < coseNeed) return;
        if (InitManager.resourceController.exp < expNeed) return;
        InitManager.resourceController.CostIncrease(-coseNeed);
        InitManager.resourceController.ExpIncrease(-expNeed);

        oc_.skillLevel[skillID]++;
        OperUIManager.skillUIController.Refresh();
    }

    /// <summary>
    /// 按下加速按钮
    /// </summary>
    public void GlobalSpeedChange()
    {
        InitManager.TimeDoubleSpeed(!InitManager.globalDoubleSpeed);
    }

    /// <summary>
    /// 按下暂停按钮
    /// </summary>
    public void GlobalPause()
    {
        InitManager.TimePause(!InitManager.globalPause);
    }
    
}
