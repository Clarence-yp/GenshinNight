using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Kroos : OperatorCore
{
    [Header("克洛丝的箭")] 
    public GameObject norArrow;
    public GameObject pyroArrow;
    public GameObject norHitAnim;

    private float[] skill1Atk = {1.05f, 1.1f, 1.2f, 1.25f, 1.3f, 1.4f, 1.5f};
    private float[] skill2Atk = {1.8f, 2f, 2.2f, 2.4f, 2.6f, 2.8f, 3f};
    private float[] talent1Probability = {0.1f, 0.2f, 0.2f};
    private float talent1Rate = 1.5f;
    
    private bool canAttachElement = false;

    private void talent1(ref float multi)
    {
        float x = Random.Range(0f, 1f);
        if (x <= talent1Probability[eliteLevel])
            multi *= talent1Rate;
    }

    IEnumerator Skill1()
    {
        float multi = skill1Atk[skillLevel[0]];
        talent1(ref multi);
        
        sp_.ReleaseSkill();
        Archery(multi, norArrow, NorAttack);
        yield return new WaitForSeconds(0.1f);
        Archery(multi, norArrow, NorAttack);
    }

    public override void SkillAtk_2()
    {
        float multi = skill2Atk[skillLevel[1]];
        multi *= talent1Rate;
        
        sp_.ReleaseSkill();
        Archery(multi, pyroArrow, PyroAttack);
    }


    public override void OnAttack()
    {
        if (skillNum == 0 && sp_.outType == releaseType.atk && sp_.CanReleaseSkill())
        {
            StartCoroutine(Skill1());
        }
        else if (skillNum == 1 && sp_.outType == releaseType.atk && sp_.CanReleaseSkill())
        {
            SkillAtk_2();
        }
        else
        {
            sp_.GetSp_Atk();
            Archery(1f, norArrow, NorAttack);
        }
        
    }

    private void Archery(float multi, GameObject proArrow, Action<float, BattleCore, parabola> endAttack)
    {// 射一支箭出去，攻击倍率为multi
        
        var arrow = PoolManager.GetObj(proArrow);
        parabola par = arrow.GetComponent<parabola>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        par.Init(pos, this, target, 12f, endAttack, multi);

        if (proArrow == pyroArrow) canAttachElement = defaultElementTimer.AttachElement(target);
    }

    private void NorAttack(float multi, BattleCore tarBC, parabola par)
    {
        GameObject hitAnim = PoolManager.GetObj(norHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        ElementSlot elementSlot = new ElementSlot();
        bool haveText = multi > 1;
        Battle(tarBC, atk_.val * multi, DamageMode.Physical, elementSlot,
            defaultElementTimer, haveText);
    }
    
    private void PyroAttack(float multi, BattleCore tarBC, parabola par)
    {
        GameObject hitAnim = PoolManager.GetObj(norHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        ElementSlot elementSlot = new ElementSlot(ElementType.Pyro, 2f);
        bool haveText = multi > 1;
        Battle(tarBC, atk_.val * multi, DamageMode.Physical, elementSlot,
            canAttachElement, haveText);
        canAttachElement = false;
    }


    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        if (SkillID == 0)
        {
            return "克洛丝的下次攻击会连续射击2次，每次射击造成攻击力" +
                   ColorfulText.ChangeToColorfulPercentage(skill1Atk[lel], ColorfulText.normalBlue)
                   + "的普通物理伤害";
        }
        else
        {
            return "克洛丝的下次攻击会发射一支充满火元素的箭矢，造成攻击力" +
                   ColorfulText.ChangeToColorfulPercentage(skill2Atk[lel], ColorfulText.normalBlue)
                   + "的" +
                   ColorfulText.GetColorfulText("火元素物理", ColorfulText.PyroRed)
                   + "伤害\n"+
                   ColorfulText.GetColorfulText("2", ColorfulText.normalBlue)
                   + "单位火元素附着量，"+
                   ColorfulText.GetColorfulText("3", ColorfulText.normalBlue)
                   +"秒独立元素附着计时器\n"
                   +"本次攻击必定触发天赋的暴击效果";
        }
    }

    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return "攻击时，" +
                   ColorfulText.ChangeToPercentage(talent1Probability[eliteLevel]) +
                   "几率当次攻击的攻击力提升至150%";
        }
        else
        {
            return "";
        }
    }
}