using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : OperatorCore
{
    [Header("空爆的特效")] 
    public GameObject norArrow;
    public GameObject anemoArrow;
    public GameObject norBoom;
    public GameObject anemoBoom;
    
    private float[] boomRangeIncrease = {0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f};
    private float[] skill2Multi = {1.4f, 1.6f, 1.9f, 2.2f, 2.5f, 2.8f, 3f};

    private Dictionary<parabola, float> rangeDic = new Dictionary<parabola, float>();
    private ValueBuffer boomRange = new ValueBuffer(1);


    public override void SkillStart_1()
    {
        float rangeInc = boomRangeIncrease[skillLevel[0]];
        SkillValueBuff valueBuff = new SkillValueBuff(boomRange, ValueBuffMode.Percentage, rangeInc, this);
        BuffManager.AddBuff(valueBuff);
        
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
            Archery(skill2Multi[skillLevel[1]], boomRange.val, anemoArrow, AnemoAttack);
            sp_.ReleaseSkill();
        }
        else
        {
            sp_.GetSp_Atk();
            Archery(1f, boomRange.val, norArrow, NorAttack);
        }
    }
    
    
    private void Archery(float multi, float range, GameObject proArrow, 
        Action<float, BattleCore, parabola> endAttack)
    {// 射一支箭出去，攻击倍率为multi
        
        var arrow = PoolManager.GetObj(proArrow);
        parabola par = arrow.GetComponent<parabola>();

        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        par.Init(pos, this, target, 12f, endAttack, multi);
        rangeDic.Add(par,range);
    }

    private void NorAttack(float multi, BattleCore tarBC, parabola par)
    {
        GameObject hitAnim = PoolManager.GetObj(norBoom);
        Vector3 pos = tarBC.transform.position;
        pos.y = 0;
        hitAnim.transform.position = pos;
        float range = rangeDic[par];
        rangeDic.Remove(par);
        Vector3 scale = hitAnim.transform.localScale;
        scale *= range;
        Scale_DurationRecycleObj recycleObj = new Scale_DurationRecycleObj(hitAnim, scale, 1f);
        BuffManager.AddBuff(recycleObj);

        
        Vector3 center = tarBC.transform.position;
        var tars = InitManager.GetNearByEnemy(center, range);
        ElementSlot elementSlot = new ElementSlot();
        foreach (var tar in tars)
        {
            bool haveText = range > 1;
            Battle(tar, atk_.val * multi, DamageMode.Physical, elementSlot,
                defaultElementTimer, haveText);
        }
    }
    
    private void AnemoAttack(float multi, BattleCore tarBC, parabola par)
    {
        GameObject hitAnim = PoolManager.GetObj(anemoBoom);
        Vector3 pos = tarBC.transform.position;
        pos.y = 0;
        hitAnim.transform.position = pos;
        float range = rangeDic[par];
        rangeDic.Remove(par);
        Vector3 scale = hitAnim.transform.localScale;
        scale *= range;
        Scale_DurationRecycleObj recycleObj = new Scale_DurationRecycleObj(hitAnim, scale, 1f);
        BuffManager.AddBuff(recycleObj);

        
        Vector3 center = tarBC.transform.position;
        var tars = InitManager.GetNearByEnemy(center, range);
        ElementSlot elementSlot = new ElementSlot(ElementType.Anemo, 1f);
        foreach (var tar in tars)
        {
            Battle(tar, atk_.val * multi, DamageMode.Physical, elementSlot,
                defaultElementTimer, true);
        }
    }

    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        if (SkillID == 0)
        {
            return "普通攻击的爆炸范围提升至" +
                   ColorfulText.ChangeToColorfulPercentage(boomRangeIncrease[lel] + 1, ColorfulText.normalBlue);
        }
        else
        {
            return "空爆的下一次攻击将发射一枚由风元素凝聚而成的弹头，造成攻击力" +
                   ColorfulText.ChangeToColorfulPercentage(skill2Multi[lel], ColorfulText.normalBlue)
                   + "的范围" +
                   ColorfulText.GetColorfulText("风元素物理", ColorfulText.AnemoGreen)
                   + "伤害\n" +
                   ColorfulText.GetColorfulText("1", ColorfulText.normalBlue)
                   + "单位风元素附着量，" +
                   ColorfulText.GetColorfulText("3", ColorfulText.normalBlue)
                   + "秒元素附着计时器\n";
        }
    }

    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            return "攻击造成范围伤害";
        }
        else
        {
            return "";
        }
    }
    
    

}

public class Scale_DurationRecycleObj : DurationRecycleObj
{
    private Vector3 scale;
    private Vector3 preScale;

    public Scale_DurationRecycleObj(GameObject obj_, Vector3 scale_, float durTime
        , BattleCore prt = null, bool havePrt_ = false) : base(obj_, durTime, prt, havePrt_)
    {
        scale = scale_;
    }

    public override void BuffStart()
    {
        base.BuffStart();
        preScale = obj.transform.localScale;
        obj.transform.localScale = scale;
    }

    public override void BuffEnd()
    {
        base.BuffEnd();
        obj.transform.localScale = preScale;
    }
}
