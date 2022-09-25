using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Steward : OperatorCore
{
    [Header("史都华德的特效")]
    public GameObject norArrow;
    public GameObject magicHitAnim;
    public GameObject iceFog;
    
    private BattleCore tarBattleCore;
    private Vector3 fogPos;
    private GameObject fogObj;

    private float[] Skill_1_Multi = {1.5f, 1.7f, 1.9f, 2.1f, 2.4f, 2.7f, 3f};
    private float[] FogMulti = {0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f};
    private float fogRadius = 1.5f;

    protected override int enemyCmp(BattleCore a, BattleCore b)
    {
        // 给攻击范围内的敌人排序，史都华德按照防御力从大到小排
        if (a.def_.val > b.def_.val) return -1;
        if (a.def_.val < b.def_.val) return 1;
        return 0;
    }

    IEnumerator IceFog_Process()
    {
        fogObj = PoolManager.GetObj(iceFog);
        fogObj.transform.position = fogPos;
        yield return new WaitForSeconds(0.7f);
        FogCauseDamage(fogObj.transform.position);
        yield return new WaitForSeconds(1.2f);
        FogCauseDamage(fogObj.transform.position);
        yield return new WaitForSeconds(1.2f);
        FogCauseDamage(fogObj.transform.position);
        yield return new WaitForSeconds(1.2f);
        FogCauseDamage(fogObj.transform.position);
        yield return new WaitForSeconds(0.7f);
        RecycleFogObj(null);
        DieAction -= RecycleFogObj;
    }
    
    private void RecycleFogObj(BattleCore bc_)
    {
        PoolManager.RecycleObj(fogObj);
    }
    
    private void FogCauseDamage(Vector3 pos)
    {
        var tars = InitManager.GetNearByEnemy(pos, fogRadius);
        float multi = FogMulti[skillLevel[1]];
        ElementSlot elementSlot = new ElementSlot(ElementType.Cryo, 1f);
        
        foreach (var ec_ in tars)
        {
            Battle(ec_, atk_.val * multi, DamageMode.Magic, elementSlot, 
                defaultElementTimer, true);
        }
    }
    
    
    IEnumerator ReturnToSta0()
    {
        while (true)
        {
            if (!sp_.during)
            {
                anim.SetInteger("sta",0);
                break;
            }
            yield return null;
        }
    }
    
    public override void SkillStart_2()
    {
        anim.SetInteger("sta",1);
        sp_.ReleaseSkill();
        StartCoroutine(ReturnToSta0());
    }


    public override void OnAttack()
    {
        if (skillNum == 1 && sp_.during)
        {
            // 创建一团冰雾，并开始造成伤害
            fogPos = GetFogPos();
            StartCoroutine(IceFog_Process());
            DieAction += RecycleFogObj;
        }
        else if(skillNum == 0 && sp_.outType == releaseType.atk && sp_.CanReleaseSkill())
        {
            float multi = Skill_1_Multi[skillLevel[0]];
            Archery(multi, NorAttack);
            sp_.ReleaseSkill();
        }
        else
        {
            Archery(1, NorAttack);
            sp_.GetSp_Atk();
        }
    }

    private void Archery(float multi, Action<float, BattleCore> endAttack)
    {// 射一支箭出去，攻击倍率为multi
        
        var arrow = PoolManager.GetObj(norArrow);
        TrackMove tm = arrow.GetComponent<TrackMove>();
        
        tarBattleCore = target;
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        tm.Init(pos, this, tarBattleCore, 10, endAttack, multi);
    }

    private Vector3 GetFogPos()
    {
        Vector3 pos = transform.position;
        
        if (!tarIsNull)
        {
            pos = target.transform.position;
            pos.y = 0;
            return pos;
        }
        
        switch (atkRange.transform.eulerAngles.y)
        {
            case 0:
                pos.x += 2;
                break;
            case 90:
                pos.z -= 2;
                break;
            case 180:
                pos.x -= 2;
                break;
            case -90:
                pos.z += 2;
                break;
        }

        pos.y = 0;
        return pos;
    }
    
    private void NorAttack(float multi, BattleCore tarBC)
    {
        GameObject hitAnim = PoolManager.GetObj(magicHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 2f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        ElementSlot elementSlot = new ElementSlot();
        bool haveText = multi > 1;
        Battle(tarBC, atk_.val * multi, DamageMode.Magic, elementSlot,
            defaultElementTimer, haveText);
    }

    public override string GetSkillDescription(int SkillID)
    {
        if (SkillID == 0)
        {
            return "下次攻击的攻击力提高至" +
                   ColorfulText.ChangeToColorfulPercentage(Skill_1_Multi[skillLevel[0]], ColorfulText.normalBlue);
        }
        else
        {
            return "史都华德凝聚记忆中的风雪，在目标脚下召唤一片冰雾区。冰雾会持续对其中的敌人造成" +
                   ColorfulText.GetColorfulText("冰元素魔法", ColorfulText.CryoWhite) +
                   "伤害，每次伤害的倍率为" +
                   ColorfulText.ChangeToColorfulPercentage(FogMulti[skillLevel[1]], ColorfulText.normalBlue) +
                   "攻击力，冰雾持续5秒\n如果当前没有目标，史都华德会选择在正前方召唤冰雾\n" +
                   ColorfulText.GetColorfulText("1", ColorfulText.normalBlue)
                   + "单位冰元素附着量，" +
                   ColorfulText.GetColorfulText("3", ColorfulText.normalBlue)
                   + "秒元素附着计时器\n";
        }
    }
}
