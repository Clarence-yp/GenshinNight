using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orchid : OperatorCore
{
    [Header("梓兰的特效")]
    public GameObject norArrow;
    public GameObject hydroArrow;
    public GameObject magicHitAnim;

    private float[] AtkIncrease = {0.1f, 0.14f, 0.19f, 0.25f, 0.32f, 0.4f, 0.5f};
    private float[] AtkSpeedIncrease = {10, 14, 19, 25, 32, 40, 50};
    private float[] Skill2Multi = {1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f};
    private float slowRate = 0.2f;
    private float slowDuration = 0.8f;

    public override void SkillStart_1()
    {
        float atkInc = AtkIncrease[skillLevel[0]];
        SkillValueBuff atkBuff = new SkillValueBuff(atk_, ValueBuffMode.Percentage, atkInc, sp_);
        BuffManager.AddBuff(atkBuff);
        
        float atkSpeedInc = AtkSpeedIncrease[skillLevel[0]];
        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(atkSpeedController, atkSpeedInc, sp_);
        BuffManager.AddBuff(atkSpeedBuff);
        
        GameObject light = PoolManager.GetObj(StoreHouse.instance.underGroundLight);
        Vector3 pos = new Vector3(0, 0, -0.2f);
        light.transform.SetParent(transform);
        light.transform.localPosition = pos;
        SkillRecycleObj recycleObj = new SkillRecycleObj(light, this);
        BuffManager.AddBuff(recycleObj);
    }


    public override void OnAttack()
    {
        if (skillNum == 1 && sp_.outType == releaseType.atk && sp_.CanReleaseSkill())
        {
            float multi = Skill2Multi[skillLevel[1]];

            for (int i = 0; i < enemyList.Count && i < 3; i++)
            {
                Archery(enemyList[i], multi, hydroArrow, HydroAttack);
            }
            
            sp_.ReleaseSkill();
        }
        else
        {
            sp_.GetSp_Atk();
            Archery(target, 1f, norArrow, NorAttack);
        }
    }
    
    private void Archery(BattleCore tarBC, float multi, GameObject proArrow, 
        Action<float, BattleCore, TrackMove> endAttack)
    {// 射一支箭出去，攻击倍率为multi
        
        var arrow = PoolManager.GetObj(proArrow);
        TrackMove tm = arrow.GetComponent<TrackMove>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        tm.Init(pos, this, tarBC, 12f, endAttack, multi);
    }
    
    private void NorAttack(float multi, BattleCore tarBC, TrackMove tm)
    {
        GameObject hitAnim = PoolManager.GetObj(magicHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        EnemySlowDurationBuff slowBuff = new EnemySlowDurationBuff(
            (EnemyCore)tarBC, slowRate, slowDuration);
        BuffManager.AddBuff(slowBuff);
        
        ElementSlot elementSlot = new ElementSlot();
        Battle(tarBC, atk_.val * multi, DamageMode.Magic, elementSlot,
            defaultElementTimer, false);
    }

    private void HydroAttack(float multi, BattleCore tarBC, TrackMove tm)
    {
        GameObject hitAnim = PoolManager.GetObj(magicHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        EnemySlowDurationBuff slowBuff = new EnemySlowDurationBuff(
            (EnemyCore)tarBC, slowRate, slowDuration);
        BuffManager.AddBuff(slowBuff);

        ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 1f);
        Battle(tarBC, atk_.val * multi, DamageMode.Magic, elementSlot,
            defaultElementTimer, true);
    }
    
    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        if (SkillID == 0)
        {
            return "攻击力+" +
                   ColorfulText.ChangeToColorfulPercentage(AtkIncrease[lel])
                   + "，攻击速度+" +
                   ColorfulText.GetColorfulText(AtkSpeedIncrease[lel].ToString("f0"), ColorfulText.normalBlue);
        }
        else
        {
            return "梓兰的下次攻击会对攻击范围内最多3个目标发射魔法水弹，造成攻击力" +
                   ColorfulText.ChangeToColorfulPercentage(Skill2Multi[lel])
                   + "的" +
                   ColorfulText.GetColorfulText("水元素魔法", ColorfulText.HydroBlue)
                   + "伤害\n"+
                   ColorfulText.GetColorfulText("1", ColorfulText.normalBlue)
                   + "单位水元素附着量，"+
                   ColorfulText.GetColorfulText("3", ColorfulText.normalBlue)
                   +"秒独立元素附着计时器\n"
                   +"本次攻击同样能触发天赋的停顿效果";
        }
    }
    
    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return "梓兰的攻击能对敌人造成短暂的停顿，使敌人的移动速度降低" +
                   ColorfulText.ChangeToColorfulPercentage(1 - slowRate) +
                   "，持续" +
                   ColorfulText.GetColorfulText(slowDuration.ToString("f1"), ColorfulText.normalBlue)
                   + "秒";
        }
        else
        {
            return "";
        }
    }
    
}
