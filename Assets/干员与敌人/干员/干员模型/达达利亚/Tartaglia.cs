using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tartaglia : OperatorCore
{
    [Header("达达利亚的特效")] 
    public GameObject norArrow;
    public GameObject hydroArrow;
    public GameObject norHitAnim;
    public GameObject aimHitAnim;
    public GameObject underWater;
    public GameObject gatherOnBow;
    public GameObject hydroBomb;
    public GameObject riptideImage;
    public GameObject riptideFlashAnim;
    public GameObject riptideSlashAnim;
    public GameObject riptideBurstAnim;

    private ElementTimer noCoolDownTimer;
    private ElementTimer riptideTimer;

    // 断流
    
    
    // 技能1
    private float[] skill1_Multi = {1.5f, 1.7f, 1.9f, 2.1f, 2.4f, 2.7f, 3f};
    private float[] riptideFlash_Multi = {0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.5f};
    
    
    // 技能2
    private float[] skill2_Multi = {1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f};
    private float[] skill2_atkSpeedIncrease = {0, 0, 0, 0, 0, 0, 0};
    private float[] riptideSlash_Multi = {0.5f, 0.55f, 0.6f, 0.68f, 0.73f, 0.8f, 0.9f};
    
    // 技能3
    private float[] skill3_BigMulti = {4f, 4.6f, 5.2f, 5.8f, 6.4f, 7.2f, 8f};
    private float[] skill3_Multi = {2f, 2.3f, 2.6f, 2.9f, 3.2f, 3.6f, 4f};
    private float[] skill3_atkSpeedIncrease = {-30, -30, -30, -30, -30, -30, -30};
    
    private float riptideBurst_Multi = 0.7f;
    
    public Vector3 skill3_tarPos;
    private float skill3_radius = 1.2f;


    protected override void Awake_Core()
    {
        base.Awake_Core();
        noCoolDownTimer = new ElementTimer(this, -1);
        riptideTimer = new ElementTimer(this, 2f);
    }

    public override void SkillStart_3()
    {
        TartagliaSkill3 skill3 = new TartagliaSkill3(this);
        BuffManager.AddBuff(skill3);
    }

    public void Skill3_Begin()
    {
        Vector3 scale = new Vector3(ac_.dirRight ? 1 : -1, 1, 1);
        
        GameObject underWater_ins = PoolManager.GetObj(underWater);
        underWater_ins.transform.SetParent(transform);
        Vector3 pos = Vector3.zero;
        pos.y = 0.01f;
        underWater_ins.transform.localPosition = pos;
        Scale_DurationRecycleObj recycleUnderWater = new Scale_DurationRecycleObj(
            underWater_ins, scale, 1.2f, this, true);
        BuffManager.AddBuff(recycleUnderWater);
        
        
        GameObject gatherOnBow_ins = PoolManager.GetObj(gatherOnBow);
        gatherOnBow_ins.transform.SetParent(frontCanvas.transform);
        pos = Vector3.zero;
        gatherOnBow_ins.transform.localPosition = pos;
        Scale_DurationRecycleObj recycleGatherOnBow = new Scale_DurationRecycleObj(
            gatherOnBow_ins, scale, 1.2f, this, true);
        BuffManager.AddBuff(recycleGatherOnBow);

        
        GameObject hydroBomb_ins = PoolManager.GetObj(hydroBomb);

        TartagliaSkill3HydroBoom hb = new TartagliaSkill3HydroBoom(
            this, hydroBomb_ins, (EnemyCore) target);
        BuffManager.AddBuff(hb);
    }
    
    public override void SkillAtk_3()
    {
        
    }

    public void Skill3CauseDamage()
    {
        var tars = InitManager.GetNearByEnemy(skill3_tarPos, skill3_radius);
        ElementSlot hydroElement = new ElementSlot(ElementType.Hydro, 1f);
        foreach (var ec_ in tars)
        {
            Battle(ec_, 100f, DamageMode.Physical, hydroElement,
                riptideTimer, true);
        }
    }


    public override void OnAttack()
    {
        if (skillNum == 0 && sp_.outType == releaseType.atk && sp_.CanReleaseSkill())
        {
            Archery(skill1_Multi[skillLevel[0]], hydroArrow, hydroAttack);
            sp_.ReleaseSkill();
        }
        else
        {
            sp_.GetSp_Atk();
            Archery(1f, norArrow, norAttack);
        }
    }
    
    private void Archery(float multi, GameObject proArrow, Action<float, BattleCore, parabola> endAttack)
    {// 射一支箭出去，攻击倍率为multi
        
        var arrow = PoolManager.GetObj(proArrow);
        parabola par = arrow.GetComponent<parabola>();
        
        Vector3 pos = transform.position;
        pos += new Vector3(ac_.dirRight ? 0.6f : -0.6f, 0.5f, 0.35f);
        par.Init(pos, this, target, 12f, endAttack, multi);
    }

    private void norAttack(float multi, BattleCore tarBC, parabola par)
    {
        GameObject hitAnim = PoolManager.GetObj(norHitAnim);
        hitAnim.transform.parent = tarBC.transform;
        Vector3 pos = new Vector3(0, 0, 0.3f);
        hitAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);
        
        Battle(tarBC, atk_.val, DamageMode.Physical);
    }
    
    private void hydroAttack(float multi, BattleCore tarBC, parabola par)
    {
        GameObject hitAnim = PoolManager.GetObj(aimHitAnim);
        hitAnim.transform.SetParent(tarBC.frontCanvas.transform);
        Vector3 pos = tarBC.transform.position;
        pos.z += 0.3f;
        hitAnim.transform.position = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 2f);
        Battle(tarBC, atk_.val, DamageMode.Physical, elementSlot, noCoolDownTimer);
    }


    public override string GetSkillDescription(int SkillID)
    {
        int lel = skillLevel[SkillID];
        switch (SkillID)
        {
            case 0:
                return "达达利亚的下一次攻击将造成" +
                       ColorfulText.ChangeToColorfulPercentage(skill1_Multi[lel]) +
                       "攻击力的" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害\n\n本次攻击" +
                       ColorfulText.GetColorfulText("必定", ColorfulText.normalBlue) +
                       "给敌人附加断流效果\n\n若本次攻击命中处于断流状态下的敌人，则会消耗断流效果,并触发" +
                       ColorfulText.GetColorfulText("断流闪", ColorfulText.normalBlue) +
                       "\n" +
                       ColorfulText.GetColorfulText("断流闪", ColorfulText.normalBlue) +
                       "：在目标周围连续造成3次" +
                       ColorfulText.ChangeToColorfulPercentage(riptideFlash_Multi[lel]) +
                       "攻击力的" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害";
            case 1:
                return "解放纯水构成的武装，进入魔王武装形态\n攻击范围" +
                       ColorfulText.GetColorfulText("缩小", ColorfulText.normalRed) +
                       "，攻击速度+" +
                       ColorfulText.GetColorfulText(skill2_atkSpeedIncrease[lel].ToString("f0"),
                           ColorfulText.normalBlue) +
                       "，同时攻击范围内的所有敌人。每次攻击造成" +
                       ColorfulText.ChangeToColorfulPercentage(skill2_Multi[lel]) +
                       "攻击力的" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害\n\n断流的附着概率变为" +
                       ColorfulText.GetColorfulText("30%", ColorfulText.normalBlue) +
                       "\n\n该状态下的攻击命中处于断流状态下的敌人时，会消耗断流效果，并触发" +
                       ColorfulText.GetColorfulText("断流斩", ColorfulText.normalBlue) +
                       "\n" +
                       ColorfulText.GetColorfulText("断流斩", ColorfulText.normalBlue) +
                       "：在目标周围造成" +
                       ColorfulText.ChangeToColorfulPercentage(riptideSlash_Multi[lel]) +
                       "攻击力的" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害";
            default:
                return "达达利亚拉起大弓，引导“魔弹一闪”攻击敌人\n攻击范围" +
                       ColorfulText.GetColorfulText("扩大", ColorfulText.normalBlue) +
                       "，攻击速度" +
                       ColorfulText.GetColorfulText(skill3_atkSpeedIncrease[lel].ToString("f0"),
                           ColorfulText.normalRed) +
                       "，每次攻击造成" +
                       ColorfulText.ChangeToColorfulPercentage(skill3_Multi[lel]) +
                       "攻击力的范围" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害\n\n断流的附着概率变为" +
                       ColorfulText.GetColorfulText("50%", ColorfulText.normalBlue) +
                       "\n\n该状态下的攻击命中处于断流状态下的敌人时，会消耗断流效果，并触发" +
                       ColorfulText.GetColorfulText("断流爆", ColorfulText.normalBlue) +
                       "\n" +
                       ColorfulText.GetColorfulText("断流爆", ColorfulText.normalBlue) +
                       "：本次攻击造成的伤害提高" +
                       ColorfulText.GetColorfulText("50%", ColorfulText.normalBlue);
            
        }
    }

    public override string GetTalentDescription(int talentID)
    {
        if (talentID == 1)
        {
            string percentage = null;
            switch (skillNum)
            {
                case 0:
                    percentage = ColorfulText.GetColorfulText("100%", ColorfulText.normalBlue);
                    break;
                case 1:
                    percentage = ColorfulText.GetColorfulText("30%", ColorfulText.normalBlue);
                    break;
                default:
                    percentage = ColorfulText.GetColorfulText("50%", ColorfulText.normalBlue);
                    break;
            }

            return "达达利亚的技能命中敌人时，有" +
                    percentage+
                   "的概率为敌人附加断流效果\n\n" +
                   "若技能命中已处于断流状态下的敌人，则会根据选择技能的不同，触发不同的效果";
        }
        else
        {
            if (eliteLevel < 2) return "";
            return "若敌人在断流状态下死亡，则会触发" +
                   ColorfulText.GetColorfulText("断流破", ColorfulText.normalBlue) +
                   "\n" +
                   ColorfulText.GetColorfulText("断流破", ColorfulText.normalBlue) +
                   "：在目标周围造成" +
                   ColorfulText.ChangeToColorfulPercentage(riptideBurst_Multi) +
                   "攻击力的" +
                   ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                   "伤害";
        }
    }
}

public class TartagliaSkill3 : SkillBuffSlot
{
    private Animator anim;
    private Tartaglia tartaglia;

    public TartagliaSkill3(Tartaglia tartaglia_) : base(tartaglia_)
    {
        tartaglia = tartaglia_;
        anim = tartaglia.anim;
    }

    public override void BuffStart()
    {
        base.BuffStart();
        anim.SetInteger("sta", 2);
        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(
            tartaglia.atkSpeedController, -30, tartaglia);
        BuffManager.AddBuff(atkSpeedBuff);
    }

    public override void BuffUpdate() { }

    public override void BuffEnd()
    {
        base.BuffEnd();
        anim.SetInteger("sta", 0);
    }
}

public class TartagliaSkill3HydroBoom : BuffSlot
{
    private float fixedTime;
    private float recycleTime;
    private GameObject hydroBoom;
    private EnemyCore tarEC;
    private Tartaglia tartaglia;

    private bool isDie;
    private bool isFixed;
    private Vector3 endPos;
    // private Vector3 preScale;
    
    public TartagliaSkill3HydroBoom(Tartaglia tarta, GameObject boom,EnemyCore tarEc)
    {
        tartaglia = tarta;
        hydroBoom = boom;
        tarEC = tarEc;

        fixedTime = 0.86f;
        recycleTime = 2.1f;

        // preScale = boom.transform.localScale;
        isDie = isFixed = false;

        // if (tarEC == null)
        // {
        //     isDie = true;
        //     endPos = tartaglia.transform.position;
        //     endPos.y = 0;
        //     hydroBoom.transform.position = endPos;
        //     hydroBoom.transform.SetParent(OperUIManager.WorldCanvas.transform);
        //     hydroBoom.transform.localScale *= 2.5f;
        // }
        // else
        if (tarEC.dying)
        {
            isDie = true;
            endPos = tarEC.animTransform.position;
            endPos.y = 0;
            hydroBoom.transform.position = endPos;
            hydroBoom.transform.SetParent(OperUIManager.WorldCanvas.transform);
        }
    }

    public override void BuffStart()
    {
        if (isDie) return;
        Vector3 pos = tarEC.transform.position;
        pos.y = 0;
        hydroBoom.transform.position = pos;
        hydroBoom.transform.SetParent(tarEC.frontCanvas.transform);
        tarEC.DieAction += Die;
    }

    public override void BuffUpdate()
    {
        if (!isFixed && (isDie && fixedTime > 0 || fixedTime <= 0))
        {
            isFixed = true;
            hydroBoom.transform.SetParent(OperUIManager.WorldCanvas.transform);
        }
        
        endPos = hydroBoom.transform.position;
        tartaglia.skill3_tarPos = endPos;
        fixedTime -= Time.deltaTime;
        recycleTime -= Time.deltaTime;
    }

    public override bool BuffEndCondition()
    {
        return recycleTime <= 0;
    }

    public override void BuffEnd()
    {
        // hydroBoom.transform.localScale = preScale;
        if (!isDie) tarEC.DieAction -= Die;
        PoolManager.RecycleObj(hydroBoom);
    }


    private void Die(BattleCore bc)
    {
        isDie = true;
    }
    
}
