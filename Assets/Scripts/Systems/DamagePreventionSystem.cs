using System;
using System.Collections;
using UnityEngine;

public class DamagePreventionSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.SubscribeReaction<DealDamageGA>(OnDealDamagePreReaction, ReactionTiming.PRE);
    }

    private void OnDisable()
    {
        ActionSystem.UnsubscribeReaction<DealDamageGA>(OnDealDamagePreReaction, ReactionTiming.PRE);
    }

    private void OnDealDamagePreReaction(DealDamageGA dealDamageGA)
    {
        foreach (var target in dealDamageGA.Targets)
        {
            ApplyDamageModification(target, dealDamageGA);
        }
    }

    private void ApplyDamageModification(CombatantView target, DealDamageGA dealDamageGA)
    {
        int currentDamage = dealDamageGA.ModifiedAmount;
        
        int armor = target.GetStatusEffectStacks(StatusEffectType.ARMOR);
        if (armor > 0)
        {
            int damageReduction = Math.Min(currentDamage, armor);
            currentDamage -= damageReduction;
            
            target.RemoveStatusEffect(StatusEffectType.ARMOR, damageReduction);
        }
        
        // Other Status Effects
        
        dealDamageGA.ModifiedAmount = currentDamage;
    }
}
