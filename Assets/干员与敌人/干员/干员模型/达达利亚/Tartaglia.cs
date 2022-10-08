using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tartaglia : OperatorCore
{
    [Header("达达利亚的特效")] 
    public GameObject norArrow;
    public GameObject hydroArrow;
    public GameObject norHitAnim;
    public GameObject aimHitAnim;
    public GameObject skill3HitAnim;
    public GameObject underWater;
    public GameObject gatherOnBow;
    public GameObject hydroBomb;
    public GameObject riptideImage;
    public GameObject riptideFlashAnim;
    public GameObject riptideSlashAnim;
    public GameObject riptideBurstAnim;
    public GameObject Skill2_AtkRange;
    public GameObject Skill3_AtkRange;
    
    private ElementTimer riptideTimer;
    private ElementTimer riptideBurstTimer;
    private ElementTimer riptideSlash_CauseTimer;

    // 断流
    public Dictionary<EnemyCore, TartagliaRiptideSlot> RiptideDic = new Dictionary<EnemyCore, TartagliaRiptideSlot>();
    private float riptideDuring = 8f;
        
    
    // 技能1
    private float[] skill1_Multi = {1.5f, 1.7f, 1.9f, 2.1f, 2.4f, 2.7f, 3f};
    private float[] riptideFlash_Multi = {0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.5f};
    private float riptideFlashRadius = 0.85f;
    private float riptideProbability_1 = 1f;
    
    // 技能2
    private float[] skill2_Multi = {2f, 2.3f, 2.6f, 2.9f, 3.2f, 3.6f, 4f};
    [HideInInspector] public float[] skill2_atkSpeedIncrease = {-30, -30, -30, -30, -30, -30, -30};
    private float riptideProbability_2 = 0.5f;
    
    [HideInInspector] public Vector3 skill2_tarPos;
    private float skill2_radius = 1.2f;
    
    
    // 技能3
    private float[] skill3_Multi = {1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f};
    [HideInInspector] public float[] skill3_atkSpeedIncrease;   // start里初始化
    private float[] riptideSlash_Multi = {0.5f, 0.55f, 0.6f, 0.68f, 0.73f, 0.8f, 0.9f};
    private float riptideSlashRadius = 1.2f;
    private float riptideProbability_3 = 0.3f;
    
    private float riptideBurstRadius = 1.5f;
    private float riptideBurst_Multi = 0.8f;
    
    


    protected override void Awake_Core()
    {
        base.Awake_Core();
        
    }

    protected override void Start_Core()
    {
        base.Start_Core();
        DieAction += DieRecycleAllRiptide;
        
        riptideTimer = new ElementTimer(this, 2f);
        riptideBurstTimer = new ElementTimer(this, 2f);
        riptideSlash_CauseTimer = new ElementTimer(this, 2f);

        skill3_atkSpeedIncrease = new []{50f, 50f, 50f, 50f, 50f, 50f, 50f};
    }

    protected override void Update_Core()
    {
        base.Update_Core();
        for (int i = 0; i < RiptideDic.Count; i++)
        {// 更新断流持续时间
            var tmp = RiptideDic.ElementAt(i);
            EnemyCore ec_ = tmp.Key;
            if (tmp.Value.remainTime - Time.deltaTime <= 0)  // 如果该敌人身上的断流到时间了
            {
                RiptideDic.Remove(ec_);
                i--;
                if (eliteLevel >= 2) ec_.DieAction -= RiptideBurstWhenDie;  // 移除断流破效果
                continue;
            }

            RiptideDic[ec_].remainTime -= Time.deltaTime;
        }
    }

    private void DieRecycleAllRiptide(BattleCore bc_)
    {// 移除所有敌人身上的断流效果
        if (eliteLevel >= 2)
            foreach (var tmp in RiptideDic)
            {
                EnemyCore ec_ = tmp.Key;
                ec_.DieAction -= RiptideBurstWhenDie;
            }
        
        RiptideDic.Clear();
    }

    private void GiveRiptide(EnemyCore ec_, float probability)
    {// 有probability的概率给目标上断流
        if (Random.Range(0f, 1f) > probability) return;
        
        if (RiptideDic.ContainsKey(ec_))
        {// 如果已有断流，则更新持续时间
            RiptideDic[ec_].remainTime = riptideDuring;
            return;
        }

        GameObject riptideObj = PoolManager.GetObj(riptideImage);
        riptideObj.transform.SetParent(ec_.frontCanvas.transform);
        Vector3 pos = new Vector3(0, -0.3f, -0.3f);
        riptideObj.transform.localPosition = pos;
        RiptideRecycle recycle = new RiptideRecycle(this, riptideObj, ec_);
        BuffManager.AddBuff(recycle);

        TartagliaRiptideSlot slot = new TartagliaRiptideSlot(riptideObj, riptideDuring);
        RiptideDic.Add(ec_, slot);

        if (eliteLevel >= 2) ec_.DieAction += RiptideBurstWhenDie;
    }
    
    public override void ElitismAction1_2()
    {// 在精二时，对所有处于断流下的敌人加死亡函数，让他们在死亡时触发断流-破
        foreach (var tmp in RiptideDic)
        {
            EnemyCore ec_ = tmp.Key;
            if (!ec_.dying) ec_.DieAction += RiptideBurstWhenDie;
        }
    }

    public override void SkillStart_2()
    {
        TartagliaSkill2 skill2 = new TartagliaSkill2(this);
        BuffManager.AddBuff(skill2);
    }

    public void Skill2_Begin()
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

        TartagliaSkill2HydroBoom hb = new TartagliaSkill2HydroBoom(
            this, hydroBomb_ins, (EnemyCore) target);
        BuffManager.AddBuff(hb);
    }

    public void Skill2CauseDamage()
    {
        var tars = InitManager.GetNearByEnemy(skill2_tarPos, skill2_radius);
        ElementSlot hydroElement = new ElementSlot(ElementType.Hydro, 1f);
        float damage = atk_.val * skill2_Multi[skillLevel[1]];
        foreach (var ec_ in tars)
        {
            float multi = 1f;
            
            // 断流-爆
            if (!RiptideDic.ContainsKey(ec_)) GiveRiptide(ec_, riptideProbability_2);
            else
            {
                RiptideDic.Remove(ec_);
                multi = 1.5f;
            }

            Battle(ec_, damage * multi, DamageMode.Physical, hydroElement,
                defaultElementTimer, true, multi > 1);
        }
    }


    public override void SkillStart_3()
    {
        TartagliaSkill3 skill3 = new TartagliaSkill3(this);
        BuffManager.AddBuff(skill3);
    }
    
    public override void SkillAtk_3()
    {
        ElementSlot hydroSlot = new ElementSlot(ElementType.Hydro, 1f);
        float damage = atk_.val * skill3_Multi[skillLevel[2]];
        foreach (var bc_ in enemyList)
        {
            EnemyCore ec_ = (EnemyCore) bc_;
            Battle(bc_, damage, DamageMode.Physical, hydroSlot, defaultElementTimer, true);
            GameObject hitAnim = PoolManager.GetObj(skill3HitAnim);
            hitAnim.transform.SetParent(ec_.transform);
            Vector3 pos = new Vector3(0, 0, 0.3f);
            hitAnim.transform.localPosition = pos;
            DurationRecycleObj recycleHitAnim = new DurationRecycleObj(hitAnim, 1f, ec_, true);
            BuffManager.AddBuff(recycleHitAnim);
            
            // 断流-斩
            if (RiptideDic.ContainsKey(ec_))
            {
                if (riptideSlash_CauseTimer.AttachElement(ec_))
                {
                    GameObject riptideSlash = PoolManager.GetObj(riptideSlashAnim);
                    Vector3 center = ec_.transform.position;
                    center.y = 0;
                    riptideSlash.transform.position = center;
                    DurationRecycleObj recycleSlash = new DurationRecycleObj(riptideSlash,1f);
                    BuffManager.AddBuff(recycleSlash);

                    var tars = InitManager.GetNearByEnemy(center, riptideSlashRadius);
                    float slashDamage = atk_.val * riptideSlash_Multi[skillLevel[2]];
                    foreach (var tarEC in tars)
                    {
                        Battle(tarEC, slashDamage, DamageMode.Physical, hydroSlot, riptideTimer, true);
                    }
                }
            }
            GiveRiptide(ec_, riptideProbability_3);
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
    {// 一技能的水箭，射到了敌人身上
        GameObject hitAnim = PoolManager.GetObj(aimHitAnim);
        hitAnim.transform.SetParent(tarBC.frontCanvas.transform);
        Vector3 pos = tarBC.transform.position;
        pos.z += 0.3f;
        hitAnim.transform.position = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(hitAnim, 1f, tarBC, true);
        BuffManager.AddBuff(recycleObj);

        ElementSlot elementSlot = new ElementSlot(ElementType.Hydro, 2f);
        Battle(tarBC, atk_.val, DamageMode.Physical, elementSlot, true, true);

        // 执行断流附着，以及断流-闪
        EnemyCore tarEC = (EnemyCore) tarBC;
        if(RiptideDic.ContainsKey(tarEC))
        {
            GameObject riptideFlash = PoolManager.GetObj(riptideFlashAnim);
            Vector3 center = tarEC.transform.position;
            center.y = 0;
            riptideFlash.transform.position = center;
            DurationRecycleObj recycleFlash = new DurationRecycleObj(riptideFlash,1f);
            BuffManager.AddBuff(recycleFlash);

            var tars = InitManager.GetNearByEnemy(center, riptideFlashRadius);
            foreach (var ec_ in tars)
            {
                StartCoroutine(RiptideFlashDamage(ec_));
            }
        }
        GiveRiptide(tarEC, riptideProbability_1);
    }

    IEnumerator RiptideFlashDamage(EnemyCore ec_)
    {
        ElementSlot hydroSlot = new ElementSlot(ElementType.Hydro, 1f);
        float damage = atk_.val * riptideFlash_Multi[skillLevel[0]];
        Battle(ec_, damage, DamageMode.Physical, hydroSlot, riptideTimer, true);
        yield return new WaitForSeconds(0.15f);
        Battle(ec_, damage, DamageMode.Physical, hydroSlot, riptideTimer, true);
        yield return new WaitForSeconds(0.15f);
        Battle(ec_, damage, DamageMode.Physical, hydroSlot, riptideTimer, true);
    }

    private void RiptideBurstWhenDie(BattleCore bc_)
    {
        EnemyCore ec_ = (EnemyCore) bc_;
        GameObject riptideBurst = PoolManager.GetObj(riptideBurstAnim);
        Vector3 center = ec_.transform.position;
        center.y = 0;
        riptideBurst.transform.position = center;
        DurationRecycleObj recycleFlash = new DurationRecycleObj(riptideBurst,1f);
        BuffManager.AddBuff(recycleFlash);

        var tars = InitManager.GetNearByEnemy(center, riptideBurstRadius);
        ElementSlot hydroSlot = new ElementSlot(ElementType.Hydro, 1f);
        float damage = atk_.val * riptideBurst_Multi;
        foreach (var tarEC in tars)
        {
            Battle(tarEC, damage, DamageMode.Physical, hydroSlot, riptideBurstTimer, true);
        }
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
                       "给敌人附加断流效果\n\n若本次攻击命中处于断流状态下的敌人，则会触发" +
                       ColorfulText.GetColorfulText("断流闪", ColorfulText.normalBlue) +
                       "\n" +
                       ColorfulText.GetColorfulText("断流闪", ColorfulText.normalBlue) +
                       "：在目标周围连续造成3次" +
                       ColorfulText.ChangeToColorfulPercentage(riptideFlash_Multi[lel]) +
                       "攻击力的" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害";
            case 1:
                return "达达利亚拉起大弓，引导“魔弹一闪”攻击敌人\n攻击范围" +
                       ColorfulText.GetColorfulText("扩大", ColorfulText.normalBlue) +
                       "，攻击速度" +
                       ColorfulText.GetColorfulText(skill2_atkSpeedIncrease[lel].ToString("f0"),
                           ColorfulText.normalRed) +
                       "，每次攻击造成" +
                       ColorfulText.ChangeToColorfulPercentage(skill2_Multi[lel]) +
                       "攻击力的范围" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害\n\n断流的附着概率变为" +
                       ColorfulText.ChangeToColorfulPercentage(riptideProbability_2) +
                       "\n\n该状态下的攻击命中处于断流状态下的敌人时，会" +
                       ColorfulText.GetColorfulText("消耗", ColorfulText.normalRed) +
                       "断流效果，并触发" +
                       ColorfulText.GetColorfulText("断流爆", ColorfulText.normalBlue) +
                       "\n" +
                       ColorfulText.GetColorfulText("断流爆", ColorfulText.normalBlue) +
                       "：本次攻击造成的伤害提高" +
                       ColorfulText.GetColorfulText("50%", ColorfulText.normalBlue);
            default:
                return "解放纯水构成的武装，进入魔王武装形态\n攻击范围" +
                       ColorfulText.GetColorfulText("缩小", ColorfulText.normalRed) +
                       "，攻击速度+" +
                       ColorfulText.GetColorfulText(skill3_atkSpeedIncrease[lel].ToString("f0"),
                           ColorfulText.normalBlue) +
                       "，同时攻击范围内的所有敌人。每次攻击造成" +
                       ColorfulText.ChangeToColorfulPercentage(skill3_Multi[lel]) +
                       "攻击力的" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害\n\n断流的附着概率变为" +
                       ColorfulText.ChangeToColorfulPercentage(riptideProbability_3) +
                       "\n\n该状态下的攻击命中处于断流状态下的敌人时，会触发" +
                       ColorfulText.GetColorfulText("断流斩", ColorfulText.normalBlue) +
                       "\n" +
                       ColorfulText.GetColorfulText("断流斩", ColorfulText.normalBlue) +
                       "：在目标周围造成" +
                       ColorfulText.ChangeToColorfulPercentage(riptideSlash_Multi[lel]) +
                       "攻击力的" +
                       ColorfulText.GetColorfulText("水元素物理", ColorfulText.HydroBlue) +
                       "伤害";
            
            
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
                    percentage = ColorfulText.ChangeToColorfulPercentage(
                        riptideProbability_1, ColorfulText.normalBlue);
                    break;
                case 1:
                    percentage = ColorfulText.ChangeToColorfulPercentage(
                        riptideProbability_2, ColorfulText.normalBlue);
                    break;
                default:
                    percentage = ColorfulText.ChangeToColorfulPercentage(
                        riptideProbability_3, ColorfulText.normalBlue);
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

    public override string GetSkillAtkRangeName(int SkillID)
    {
        return SkillID switch
        {
            1 => Skill2_AtkRange.name,
            2 => Skill3_AtkRange.name,
            _ => ""
        };
    }
}

public class TartagliaRiptideSlot
{
    public GameObject riptide;
    public float remainTime;

    public TartagliaRiptideSlot(GameObject Riptide, float t)
    {
        riptide = Riptide;
        remainTime = t;
    }
}

public class RiptideRecycle : PrtRecycleObj
{
    private Tartaglia tartaglia;
    private EnemyCore target;

    public RiptideRecycle(Tartaglia tarta, GameObject riptideObj, BattleCore Prt) : base(riptideObj, Prt)
    {
        tartaglia = tarta;
        target = (EnemyCore) Prt;
    }

    public override void BuffStart() { }

    public override void BuffUpdate() { }

    public override bool BuffEndCondition()
    {
        bool contain = tartaglia.RiptideDic.ContainsKey(target);
        return !contain || base.BuffEndCondition();
    }
}

public class TartagliaSkill2 : SkillBuffSlot
{
    private Animator anim;
    private Tartaglia tartaglia;

    public TartagliaSkill2(Tartaglia tartaglia_) : base(tartaglia_)
    {
        tartaglia = tartaglia_;
        anim = tartaglia.anim;
    }

    public override void BuffStart()
    {
        base.BuffStart();
        anim.SetInteger("sta", 1);
        tartaglia.ChangeAtkRange(tartaglia.Skill2_AtkRange);
        float atkSpeedInc = tartaglia.skill2_atkSpeedIncrease[tartaglia.skillLevel[1]];
        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(
            tartaglia.atkSpeedController, atkSpeedInc, tartaglia);
        BuffManager.AddBuff(atkSpeedBuff);
    }

    public override void BuffUpdate() { }

    public override void BuffEnd()
    {
        base.BuffEnd();
        anim.SetInteger("sta", 0);
        tartaglia.ChangeAtkRange();
    }
}

public class TartagliaSkill2HydroBoom : BuffSlot
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
    
    public TartagliaSkill2HydroBoom(Tartaglia tarta, GameObject boom,EnemyCore tarEc)
    {
        tartaglia = tarta;
        hydroBoom = boom;
        tarEC = tarEc;

        fixedTime = 0.86f;
        recycleTime = 2.1f;
        
        isDie = isFixed = false;
        
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
        tartaglia.skill2_tarPos = endPos;
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
        tartaglia.ChangeAtkRange(tartaglia.Skill3_AtkRange);
        float atkSpeedInc = tartaglia.skill3_atkSpeedIncrease[tartaglia.skillLevel[2]];
        SkillAtkSpeedBuff atkSpeedBuff = new SkillAtkSpeedBuff(
            tartaglia.atkSpeedController, atkSpeedInc, tartaglia);
        BuffManager.AddBuff(atkSpeedBuff);
    }

    public override void BuffUpdate() { }

    public override void BuffEnd()
    {
        base.BuffEnd();
        anim.SetInteger("sta", 0);
        tartaglia.ChangeAtkRange();
    }
}