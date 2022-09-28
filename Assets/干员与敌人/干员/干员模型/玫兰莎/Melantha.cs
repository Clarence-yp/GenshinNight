using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melantha : OperatorCore
{
    public GameObject slashAnim;
    public GameObject cutAnim;
    private float[] AtkIncrease = {0.1f, 0.15f, 0.2f, 0.3f, 0.35f, 0.4f, 0.5f};
    private float[] elecAtkMulti = {1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 2f};
    private float skill2_r = 1.5f;
    
    private Vector3 skill2Center;

    public override void SkillStart_1()
    {
        float atkInc = AtkIncrease[skillLevel[0]];
        SkillValueBuff valueBuff = new SkillValueBuff(atk_, ValueBuffMode.Percentage, atkInc, sp_);
        BuffManager.AddBuff(valueBuff);

        GameObject light = PoolManager.GetObj(StoreHouse.instance.underGroundLight);
        Vector3 pos = new Vector3(0, 0, -0.2f);
        light.transform.SetParent(transform);
        light.transform.localPosition = pos;
        SkillRecycleObj recycleObj = new SkillRecycleObj(light, this);
        BuffManager.AddBuff(recycleObj);
    }

    public override void SkillStart_2()
    {
        MelanthaSkill2 melanthaSkill2 = new MelanthaSkill2(this);
        BuffManager.AddBuff(melanthaSkill2);
    }

    public override void SkillAtk_2()
    {
        slashAnim = PoolManager.GetObj(slashAnim);
        slash sla = slashAnim.GetComponent<slash>();
        skill2Center = sla.Init(transform, atkRange.transform.eulerAngles);
        
        DurationRecycleObj recycleObj = new DurationRecycleObj(slashAnim, 2f);
        BuffManager.AddBuff(recycleObj);
        
        StartCoroutine(Skill2_Damage());
    }

    IEnumerator Skill2_Damage()
    {
        yield return new WaitForSeconds(0.4f);
        Skill2_CauseDamage();
        yield return new WaitForSeconds(0.23f);
        Skill2_CauseDamage();
        yield return new WaitForSeconds(0.23f);
        Skill2_CauseDamage();
    }

    private void Skill2_CauseDamage()
    {
        var tars = InitManager.GetNearByEnemy(skill2Center, skill2_r);
        float multi = elecAtkMulti[skillLevel[1]];
        ElementSlot elementSlot = new ElementSlot(ElementType.Electro, 2f);

        foreach (var ec_ in tars)
        {
            Battle(ec_, atk_.val * multi, DamageMode.Physical, elementSlot, 
                defaultElementTimer, true);
            
            GameObject CutAnim = PoolManager.GetObj(cutAnim);
            CutAnim.transform.parent = ec_.transform;
            Vector3 pos = new Vector3(0, 0, 0.3f);
            CutAnim.transform.localPosition = pos;
            DurationRecycleObj recycleObj = new DurationRecycleObj(CutAnim, 1f, ec_, true);
            BuffManager.AddBuff(recycleObj);
        }
    }

    public override string GetSkillDescription(int SkillID)
    {
        if (SkillID == 0)
        {
            return "攻击力+" +
                   ColorfulText.ChangeToColorfulPercentage(AtkIncrease[skillLevel[0]], ColorfulText.normalBlue);
        }
        else
        {
            return "玫兰莎汇聚雷电的力量，快速拔刀向前方挥出多剑，造成3次" +
                   ColorfulText.GetColorfulText("雷元素物理", ColorfulText.ElectroPurple) +
                   "范围伤害，每次伤害的倍率为" +
                   ColorfulText.ChangeToColorfulPercentage(elecAtkMulti[skillLevel[1]], ColorfulText.normalBlue) +
                   "攻击力\n" +
                   ColorfulText.GetColorfulText("1", ColorfulText.normalBlue)
                   + "单位雷元素附着量，" +
                   ColorfulText.GetColorfulText("3", ColorfulText.normalBlue)
                   + "秒独立元素附着计时器\n";
        }
    }
}

public class MelanthaSkill2 : BuffSlot
{
    private Melantha melantha;
    private SPController sp_;
    private bool isDie;

    public MelanthaSkill2(Melantha prt)
    {
        melantha = prt;
        sp_ = melantha.sp_;
    }

    public override void BuffStart()
    {
        melantha.anim.SetInteger("sta", 1);
        sp_.ReleaseSkill();
        melantha.DieAction += Die;
    }

    public override void BuffUpdate() { }

    public override bool BuffEndCondition()
    {
        return !sp_.during || isDie;
    }

    public override void BuffEnd()
    {
        if (!isDie) melantha.DieAction -= Die;
        melantha.anim.SetInteger("sta", 0);
    }

    private void Die(BattleCore bc_)
    {
        isDie = true;
    }
    
    
}


