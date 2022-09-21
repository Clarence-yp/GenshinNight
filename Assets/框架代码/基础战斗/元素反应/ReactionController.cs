using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class ReactionController
{
    // 元素反应共用的计时器，由ReactionTransfer调用，主要用于扩散挂元素
    public static ElementTimer reactionTimer = new ElementTimer(null);

    private static float OverLoadRadius = 1.5f;
    private static float SuperConductRadius = 1.5f;
    private static float SuperConductDuring = 8f;
    private static float ElectroChargedInterval = 1f;
    private static Color32 FrozenColor = new Color32(50, 150, 255, 255);
        
    private ElementCore elc_;

    private float superConductTime = 0;         // 超导的减防状态还剩余多长时间
    private float electroChargedTime = 0;       // 感电反应冷却时间
    private float electroChargedMastery;        // 感电反应吃的精通，为最后一个上水雷元素的精通
    

    public ReactionController(ElementCore elementCore)
    {
        elc_ = elementCore;
    }

    public void Update()
    {
        // 超导随时间减少
        if (superConductTime > 0)
        {
            elc_.superConducting = true;
            superConductTime -= Time.deltaTime;
        }
        else
        {
            elc_.superConducting = false;
        }
        
        // 判断能否发生感电反应
        electroChargedTime -= Time.deltaTime;
        if (elc_.attachedElement.ContainsKey(ElementType.Electro) &&
            elc_.attachedElement.ContainsKey(ElementType.Hydro) && electroChargedTime <= 0)
        {
            electroChargedTime = ElectroChargedInterval;
            ElectroCharged();
        }
        
        // 判断是否处于冻结中
        if (elc_.attachedElement.ContainsKey(ElementType.Frozen))
        {
            if (!elc_.frozen)
            {
                elc_.frozen = true;
                if (elc_.transform.CompareTag("operator"))
                {
                    OperatorCore oc_ = (OperatorCore) elc_;
                    oc_.ac_.ChangeColor(FrozenColor);
                    oc_.ac_.ChangeAnimSpeed();
                }
                else if (elc_.transform.CompareTag("enemy"))
                {
                    EnemyCore ec_ = (EnemyCore) elc_;
                    ec_.ac_.ChangeColor(FrozenColor);
                    ec_.ac_.ChangeAnimSpeed();
                }
                elc_.FrozenBegin();
            }
        }
        else
        {
            if (elc_.frozen)
            {
                elc_.frozen = false;
                if (elc_.transform.CompareTag("operator"))
                {
                    OperatorCore oc_ = (OperatorCore) elc_;
                    oc_.ac_.ChangeDefaultColor();
                    oc_.ac_.ChangeAnimSpeed();
                }
                else if (elc_.transform.CompareTag("enemy"))
                {
                    EnemyCore ec_ = (EnemyCore) elc_;
                    ec_.ac_.ChangeDefaultColor();
                    ec_.ac_.ChangeAnimSpeed();
                }
                elc_.FrozenEnd();
            }
        }
        
    }
    
    
    public void Reaction(ElementCore attacker, ElementSlot element1, ElementSlot element2, 
        float mastery, ref float damage, ref bool isBig)
    {// 判断两种元素应该发生什么反应，element2为后手元素，mastery为该次反应吃的元素精通

        switch (element2.eleType)       // 后手元素，表示触发元素
        {
            case ElementType.Anemo:     // 风
                Swirl(element1, element2, mastery); // 必然为扩散反应
                break;
            case ElementType.Geo:       // 岩
                Crystallization(element1, element2, attacker, mastery);// 必然为结晶反应
                break;
            case ElementType.Pyro:      // 火
                switch (element1.eleType)
                {
                    case ElementType.Hydro:     // 水
                        Vaporize(element1, element2, ref damage, mastery);
                        isBig = true;
                        break;
                    case ElementType.Cryo:      // 冰
                    case ElementType.Frozen:    // 冻元素
                        Melt(element1, element2, ref damage, mastery);
                        isBig = true;
                        break;
                    case ElementType.Electro:   // 雷
                        Overloaded(attacker, element1, element2, mastery);
                        break;
                    case ElementType.Dendro:    // 草
                        // 后面再写
                        break;
                }
                break;
            case ElementType.Hydro:     // 水
                switch (element1.eleType)
                {
                    case ElementType.Pyro:      // 火
                        Vaporize(element1, element2, ref damage, mastery);
                        isBig = true;
                        break;
                    case ElementType.Cryo:      // 冰
                        Frozen(element1, element2, mastery);
                        break;
                    case ElementType.Electro:   // 雷
                        ElectroCharged_Refresh(mastery);
                        break;
                    case ElementType.Dendro:    // 草
                        // 后面再写
                        break;
                }
                break;
            case ElementType.Cryo:      // 冰
                switch (element1.eleType)
                {
                    case ElementType.Pyro:      // 火
                        Melt(element1, element2, ref damage, mastery);
                        isBig = true;
                        break;
                    case ElementType.Hydro:     // 水
                        Frozen(element1, element2, mastery);
                        break;
                    case ElementType.Electro:   // 雷
                        SuperConduct(element1, element2, mastery);
                        break;
                    case ElementType.Dendro:    // 草
                        // 后面再写
                        break;
                }
                break;
            case ElementType.Electro:   // 雷
                switch (element1.eleType)
                {
                    case ElementType.Pyro:      // 火
                        Overloaded(attacker, element1, element2, mastery);
                        break;
                    case ElementType.Hydro:     // 水
                        ElectroCharged_Refresh(mastery);
                        break;
                    case ElementType.Cryo:      // 冰
                    case ElementType.Frozen:    // 冻元素
                        SuperConduct(element1, element2, mastery);
                        break;
                    case ElementType.Dendro:    // 草
                        // 后面再写
                        break;
                }
                break;
            case ElementType.Dendro:    // 草
                // 后面再写
                break;
        }
    }
    
    
    private void Overloaded(ElementCore attacker, 
        ElementSlot firElement, ElementSlot sedElement, float mastery)           
    {// 超载反应
        // 以本体为圆心，半径OverLoadRadius，造成一次火元素伤害，并附加眩晕或击退
        
        Vector3 center = elc_.transform.position;
        if (elc_.transform.CompareTag("operator"))
        {// 对干员造成超载反应，造成1秒的眩晕效果
            List<OperatorCore> tars = InitManager.GetNearByOper(center, OverLoadRadius);
            foreach (var oc_ in tars)
            {// 对范围内所有干员进行操作
                ElementSlot pyroElement = new ElementSlot(ElementType.Pyro);
                oc_.GetDamage(null, OverLoadDamage(mastery), DamageMode.Physical
                    , pyroElement, false, true);

                DurationDizzyBuff dizzyBuff = new DurationDizzyBuff(oc_, 1f);
                BuffManager.AddBuff(dizzyBuff);
            }
        }
        else if(elc_.transform.CompareTag("enemy"))
        {// 对敌人造成超载反应，以本体（稍微偏向发出点）为圆心产生一次爆炸，对所有敌人造成小力击退效果
            center = attacker.transform.position - elc_.transform.position;
            center.y = 0;
            float k = Mathf.Sqrt(0.01f / (center.x * center.x + center.z * center.z));
            center = center * k + elc_.transform.position;    // 爆炸的圆心

            List<EnemyCore> tars = InitManager.GetNearByEnemy(center, OverLoadRadius);
            foreach (var ec_ in tars)
            {// 对范围内的所有敌人进行操作
                ElementSlot pyroElement = new ElementSlot(ElementType.Pyro);
                ec_.GetDamage(null, OverLoadDamage(mastery), DamageMode.Physical
                        , pyroElement, false, true);
                    
                ec_.ppc_.Push(center, PushAndPullController.littleForce);
            }
        }

        // 播放超载动画
        GameObject overLoadAnim = PoolManager.GetObj(StoreHouse.instance.overLoadAnim);
        overLoadAnim.transform.position = elc_.transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(overLoadAnim, 2f);
        BuffManager.AddBuff(recycleObj);
        
        // 展示反应文本
        ShowReactionText(center, "超载", StoreHouse.OverLoadTextColor);
        
        // 雷火按1:1结算，先手元素扣除后手元素量，后手元素被扣为0
        firElement.eleCount = Mathf.Max(0, firElement.eleCount - sedElement.eleCount);
        sedElement.eleCount = 0;
    }

    private void SuperConduct(ElementSlot firElement, ElementSlot sedElement, float mastery)         
    {// 超导反应
        // 以本体为圆心，半径SuperConductRadius，造成一次并元素伤害，并降低40%防御力，持续SuperConductDuring秒
        
        float damage = SuperConductDamage(mastery);
        Vector3 center = elc_.transform.position;
        ElementSlot cryoElement = new ElementSlot(ElementType.Cryo);
        if (elc_.transform.CompareTag("operator"))
        {
            foreach (var oc_ in InitManager.GetNearByOper(center, SuperConductRadius))
            {
                oc_.GetDamage(null, damage, DamageMode.Magic, cryoElement,
                    false, true);
                
                if (!oc_.superConducting)   // 只有不在超导状态下，新的超导才会增加buff
                {
                    SuperConductBuff valueBuff = new SuperConductBuff(oc_);
                    BuffManager.AddBuff(valueBuff);
                }
                oc_.reactionController.ResetSuperConductTime();
            }
        }
        else if (elc_.transform.CompareTag("enemy"))
        {
            foreach (var ec_ in InitManager.GetNearByEnemy(center, SuperConductRadius))
            {
                ec_.GetDamage(null, damage, DamageMode.Magic, cryoElement,
                    false, true);
                
                if (!ec_.superConducting)
                {
                    SuperConductBuff valueBuff = new SuperConductBuff(ec_);
                    BuffManager.AddBuff(valueBuff);
                }
                ec_.reactionController.ResetSuperConductTime();
            }
        }
        
        // 播放超导动画
        GameObject superConductAnim = PoolManager.GetObj(StoreHouse.instance.superConductAnim);
        superConductAnim.transform.position = center;
        DurationRecycleObj recycleObj = new DurationRecycleObj(superConductAnim, 2f);
        BuffManager.AddBuff(recycleObj);
        
        // 展示反应文本
        ShowReactionText(center, "超导", StoreHouse.SuperConductTextColor);

        // 冰雷按1:1结算，先手元素扣除后手元素量，后手元素被扣为0
        firElement.eleCount = Mathf.Max(0, firElement.eleCount - sedElement.eleCount);
        sedElement.eleCount = 0;
    }

    private void ElectroCharged_Refresh(float mastery)
    {// 后手元素过来后，更新感电反应吃的精通
        electroChargedMastery = mastery;
    }
    
    private void ElectroCharged()       
    {// 感电反应
        float damage = ElectroChargedDamage(electroChargedMastery);
        ElementSlot electro = new ElementSlot(ElementType.Electro);
        elc_.GetDamage(null, damage, DamageMode.Magic, electro, false, true);
        
        //感电动画
        GameObject electroChargedAnim = PoolManager.GetObj(StoreHouse.instance.electroChargedAnim);
        electroChargedAnim.transform.parent = elc_.transform;
        Vector3 pos = new Vector3(0, 0, 0.4f);
        electroChargedAnim.transform.localPosition = pos;
        DurationRecycleObj recycleObj = new DurationRecycleObj(electroChargedAnim, 1f
            , (BattleCore) elc_, true);
        BuffManager.AddBuff(recycleObj);
        
        // 展示反应文本
        ShowReactionText(elc_.transform.position, "感电", StoreHouse.ElectroChargedColor);

        // 水雷1:1消耗，每次反应消耗各0.4元素
        elc_.attachedElement[ElementType.Electro] -= 0.4f;
        if (elc_.attachedElement[ElementType.Electro] <= 0)
            elc_.attachedElement.Remove(ElementType.Electro);
        elc_.attachedElement[ElementType.Hydro] -= 0.4f;
        if (elc_.attachedElement[ElementType.Hydro] <= 0)
            elc_.attachedElement.Remove(ElementType.Hydro);
    }

    private void Frozen(ElementSlot firElement, ElementSlot sedElement, float mastery)       
    {// 冻结反应
        
        // 展示反应文本
        ShowReactionText(elc_.transform.position, "冻结", StoreHouse.FrozenColor);
        
        // 冰水按1:1结算，先手元素扣除后手元素量，后手元素被扣为0，生成2倍于扣除元素的冻元素
        float frozenCount = 2 * Mathf.Min(firElement.eleCount, sedElement.eleCount);
        if (elc_.attachedElement.ContainsKey(ElementType.Frozen))
        {
            elc_.attachedElement[ElementType.Frozen] = 
                Mathf.Max(elc_.attachedElement[ElementType.Frozen], frozenCount);
        }
        else
        {
            elc_.attachedElement.Add(ElementType.Frozen, frozenCount);
        }
        firElement.eleCount = Mathf.Max(0, firElement.eleCount - sedElement.eleCount);
        sedElement.eleCount = 0;
    }
    
    private void Swirl(ElementSlot firElement, ElementSlot sedElement, float mastery)                
    {// 扩散反应
        
    }
    
    private void Crystallization(ElementSlot firElement, ElementSlot sedElement, 
        ElementCore attacker, float mastery)
    {// 结晶反应
        
    }

    private void Vaporize(ElementSlot firElement, ElementSlot sedElement,
        ref float damage, float mastery)            
    {// 蒸发反应
        if (sedElement.eleType == ElementType.Hydro)
        {// 水打火，基础系数为2
            damage = damage * 2f;
            
            // 火水按1:2结算，已附着的火会被消耗后手水的2倍，而后手水会完全消失
            firElement.eleCount = Mathf.Max(0, firElement.eleCount - 2 * sedElement.eleCount);
            sedElement.eleCount = 0;
        }
        else
        {// 火打水，基础系数为1.5
            damage = damage * 1.5f;
            
            // 水火按2:1结算，已附着的水会被消耗后手火的一半，而后手火会完全消失
            firElement.eleCount = Mathf.Max(0, firElement.eleCount - 0.5f * sedElement.eleCount);
            sedElement.eleCount = 0;
        }
        
        // 展示反应文本
        ShowReactionText(elc_.transform.position, "蒸发", StoreHouse.VaporizeColor);
    }

    private void Melt(ElementSlot firElement, ElementSlot sedElement,
        ref float damage, float mastery)                
    {// 融化反应
        
        if (sedElement.eleType == ElementType.Pyro)
        {// 火打冰，基础系数为2
            damage = damage * 2f;
            
            // 火冰按1:2结算，已附着的冰会被消耗后手火的2倍，而后手火会完全消失
            firElement.eleCount = Mathf.Max(0, firElement.eleCount - 2 * sedElement.eleCount);
            sedElement.eleCount = 0;
        }
        else
        {// 冰打火，基础系数为1.5
            damage = damage * 1.5f;
            
            // 冰火按2:1结算，已附着的火会被消耗后手冰的一半，而后手冰会完全消失
            firElement.eleCount = Mathf.Max(0, firElement.eleCount - 0.5f * sedElement.eleCount);
            sedElement.eleCount = 0;
        }
        
        // 展示反应文本
        ShowReactionText(elc_.transform.position, "融化", StoreHouse.MeltColor);
    }



    private void ShowReactionText(Vector3 center, string reactionName, Color textColor)
    {
        GameObject obj = PoolManager.GetObj(StoreHouse.instance.reactionShowText);
        obj.transform.SetParent(OperUIManager.WorldCanvas.transform);
        Text text = obj.GetComponent<Text>();
        text.text = reactionName;
        text.color = textColor;
        center.y += 0.3f;
        text.transform.position = center;
    }



    private float OverLoadDamage(float mastery)
    {
        // 返回超载反应的伤害，只与元素精通相关
        return mastery;
    }
    
    private float SuperConductDamage(float mastery)
    {
        // 返回超导反应的伤害，只与元素精通相关
        return mastery;
    }

    private float ElectroChargedDamage(float mastery)
    {
        // 返回感电反应的伤害，只与元素精通相关
        return mastery;
    }
    

    
    




    public void ResetSuperConductTime()
    {
        superConductTime = SuperConductDuring;
        elc_.superConducting = true;
    }
}

public class DurationRecycleObj : DurationBuffSlot
{
    private GameObject obj;
    private bool havePrt;
    private BattleCore bc_;
    private bool isDie;

    public DurationRecycleObj(GameObject obj_, float durTime
        , BattleCore prt = null, bool havePrt_ = false) : base(durTime)
    {
        obj = obj_;
        bc_ = prt;
        havePrt = havePrt_;
        isDie = false;
    }

    public override void BuffStart()
    {
        if (havePrt) bc_.DieAction += Die;
    }

    public override bool BuffEndCondition()
    {
        return during <= 0 || isDie;
    }

    public override void BuffEnd()
    {
        if (havePrt) bc_.DieAction -= Die;
        PoolManager.RecycleObj(obj);
    }

    private void Die(BattleCore bc_)
    {
        isDie = true;
    }
}

public class SuperConductBuff : BuffSlot
{
    private static Vector3 localPos = new Vector3(0, 0, 0.5f);
    private static Vector3 localRol = new Vector3(-30, 0, 0);
    private ValueBuffer valueBuffer;
    private ValueBuffInner buffInner;
    private ElementCore elc_;
    private GameObject obj;

    private bool isDie;

    public SuperConductBuff(ElementCore elementCore)
    {
        elc_ = elementCore;
        valueBuffer = elc_.def_;
        buffInner = new ValueBuffInner(ValueBuffMode.Percentage, -0.4f);
        isDie = false;
    }

    public override void BuffStart()
    {
        valueBuffer.AddValueBuff(buffInner);
        obj = PoolManager.GetObj(StoreHouse.instance.superConductDuration);
        obj.transform.parent = elc_.transform;
        obj.transform.localPosition = localPos;
        obj.transform.eulerAngles = localRol;

        ((BattleCore) elc_).DieAction += Die;
    }

    public override void BuffUpdate() { }

    public override bool BuffEndCondition()
    {
        return !elc_.superConducting || isDie;
    }

    public override void BuffEnd()
    {
        ((BattleCore) elc_).DieAction -= Die;
        valueBuffer.DelValueBuff(buffInner);
        PoolManager.RecycleObj(obj);
    }

    private void Die(BattleCore bc_)
    {
        isDie = true;
    }
    
}