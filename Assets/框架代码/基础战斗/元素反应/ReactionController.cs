using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionController
{
    // 元素反应共用的计时器，由ReactionTransfer调用
    public static ElementTimer reactionTimer = new ElementTimer(null);
        
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
                        Overloaded(element1, element2, mastery);
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
                        Superconduct(element1, element2, mastery);
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
                        Overloaded(element1, element2, mastery);
                        break;
                    case ElementType.Hydro:     // 水
                        ElectroCharged(element1, element2, mastery);
                        break;
                    case ElementType.Cryo:      // 冰
                        Superconduct(element1, element2, mastery);
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
    
    
    private void Overloaded(ElementSlot firElement, ElementSlot sedElement, float mastery)           
    {// 超载反应


        
        // 雷火按1:1结算，先手元素扣除后手元素量，后手元素被扣为0
        firElement.eleCount = Mathf.Max(0, firElement.eleCount - sedElement.eleCount);
        sedElement.eleCount = 0;
    }

    private void Superconduct(ElementSlot firElement, ElementSlot sedElement, float mastery)         
    {// 超导反应
        
        
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
    
    
}
