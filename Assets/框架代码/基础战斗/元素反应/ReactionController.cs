using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionController
{
    // 元素反应共用的计时器，由ReactionTransfer调用
    public static ElementTimer reactionTimer = new ElementTimer(null);

    private const float OverLoadRadius = 1.2f;
    private const float SuperConductRadius = 1.2f;
    private const float SuperConductDuring = 8f;
        
    private ElementCore elc_;
    


    public ReactionController(ElementCore elementCore)
    {
        elc_ = elementCore;
    }

    public void Update()
    {
       
    }
    
    
    public void Reaction(ElementCore attacker, ElementSlot element1, ElementSlot element2, 
        float mastery, ref float damage)
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
                        break;
                    case ElementType.Cryo:      // 冰
                        Melt(element1, element2, ref damage, mastery);
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
                        break;
                    case ElementType.Cryo:      // 冰
                        Frozen(element1, element2, mastery);
                        break;
                    case ElementType.Electro:   // 雷
                        ElectroCharged(element1, element2, mastery);
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
                        ElectroCharged(element1, element2, mastery);
                        break;
                    case ElementType.Cryo:      // 冰
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
        
        if (elc_.transform.CompareTag("operator"))
        {// 对干员造成超载反应，造成1秒的眩晕效果
            List<OperatorCore> tars = InitManager.GetNearByOper(elc_.transform.position, OverLoadRadius);
            foreach (var oc_ in tars)
            {// 对范围内所有干员进行操作
                ElementSlot pyroElement = new ElementSlot(ElementType.Pyro);
                oc_.GetDamage(null, OverLoadDamage(mastery), DamageMode.Physical
                        , pyroElement, false);

                DurationDizzyBuff dizzyBuff = new DurationDizzyBuff(oc_, 1f);
                BuffManager.AddBuff(dizzyBuff);
            }
        }
        else if(elc_.transform.CompareTag("enemy"))
        {// 对敌人造成超载反应，以本体（稍微偏向发出点）为圆心产生一次爆炸，对所有敌人造成小力击退效果
            Vector3 center = attacker.transform.position - elc_.transform.position;
            center.y = 0;
            float k = Mathf.Sqrt(0.01f / (center.x * center.x + center.z * center.z));
            center = center * k + elc_.transform.position;    // 爆炸的圆心

            List<EnemyCore> tars = InitManager.GetNearByEnemy(elc_.transform.position, OverLoadRadius);
            foreach (var ec_ in tars)
            {// 对范围内的所有敌人进行操作
                ElementSlot pyroElement = new ElementSlot(ElementType.Pyro);
                ec_.GetDamage(null, OverLoadDamage(mastery), DamageMode.Physical
                        , pyroElement, false);
                    
                ec_.ppc_.Push(center, PushAndPullController.littleForce);
            }
        }

        // 播放超载动画
        GameObject overLoadAnim = PoolManager.GetObj(StoreHouse.instance.overLoadAnim);
        overLoadAnim.transform.position = elc_.transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(overLoadAnim, 2f);
        BuffManager.AddBuff(recycleObj);
        
        // 雷火按1:1结算，先手元素扣除后手元素量，后手元素被扣为0
        firElement.eleCount = Mathf.Max(0, firElement.eleCount - sedElement.eleCount);
        sedElement.eleCount = 0;
    }

    private void SuperConduct(ElementSlot firElement, ElementSlot sedElement, float mastery)         
    {// 超导反应
        // 以本体为圆心，半径SuperConductRadius，造成一次并元素伤害，并降低40%防御力，持续SuperConductDuring秒
        
        float damage = SuperConductDamage(mastery);
        ElementSlot cryoElement = new ElementSlot(ElementType.Cryo);
        if (elc_.transform.CompareTag("operator"))
        {
            foreach (var oc_ in InitManager.GetNearByOper(elc_.transform.position, SuperConductRadius))
            {
                oc_.GetDamage(null, damage, DamageMode.Magic, cryoElement, false);
                
                if (oc_.superConductTime <= 0)
                {
                    SuperConductDefBuff valueBuff = new SuperConductDefBuff(oc_);
                    BuffManager.AddBuff(valueBuff);
                }
                oc_.superConductTime = SuperConductDuring;
            }
        }
        else if (elc_.transform.CompareTag("enemy"))
        {
            foreach (var ec_ in InitManager.GetNearByEnemy(elc_.transform.position, SuperConductRadius))
            {
                ec_.GetDamage(null, damage, DamageMode.Magic, cryoElement, false);
                
                if (ec_.superConductTime <= 0)
                {
                    SuperConductDefBuff valueBuff = new SuperConductDefBuff(ec_);
                    BuffManager.AddBuff(valueBuff);
                }
                ec_.superConductTime = SuperConductDuring;
            }
        }
        
        // 播放超导动画
        GameObject superConductAnim = PoolManager.GetObj(StoreHouse.instance.superConductAnim);
        superConductAnim.transform.position = elc_.transform.position;
        DurationRecycleObj recycleObj = new DurationRecycleObj(superConductAnim, 2f);
        BuffManager.AddBuff(recycleObj);

        // 冰雷按1:1结算，先手元素扣除后手元素量，后手元素被扣为0
        firElement.eleCount = Mathf.Max(0, firElement.eleCount - sedElement.eleCount);
        sedElement.eleCount = 0;
    }

    private void ElectroCharged(ElementSlot firElement, ElementSlot sedElement, float mastery)       
    {// 感电反应
        
    }

    private void Frozen(ElementSlot firElement, ElementSlot sedElement, float mastery)       
    {// 冻结反应
        
        
        
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
        
        
    }

    private void Melt(ElementSlot firElement, ElementSlot sedElement,
        ref float damage, float mastery)                
    {// 融化反应
        
        
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
}

public class DurationRecycleObj : DurationBuffSlot
{
    private GameObject obj;

    public DurationRecycleObj(GameObject obj_, float durTime) : base(durTime)
    {
        obj = obj_;
    }

    public override void BuffStart() { }

    public override void BuffEnd()
    {
        PoolManager.RecycleObj(obj);
    }
}

public class SuperConductDefBuff : BuffSlot
{
    private ValueBuffer valueBuffer;
    private ValueBuffInner buffInner;
    private ElementCore elc_;

    public SuperConductDefBuff(ElementCore elementCore)
    {
        elc_ = elementCore;
        valueBuffer = elc_.def_;
        buffInner = new ValueBuffInner(ValueBuffMode.Percentage, -0.4f);
    }

    public override void BuffStart()
    {
        valueBuffer.AddValueBuff(buffInner);
    }

    public override void BuffUpdate() { }

    public override bool BuffEndCondition()
    {
        return elc_.superConductTime <= 0;
    }

    public override void BuffEnd()
    {
        valueBuffer.DelValueBuff(buffInner);
    }
}