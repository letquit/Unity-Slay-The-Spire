using System.Collections.Generic;
using UnityEngine;

public class DealDamageGA : GameAction, IHaveCaster
{
    public int Amount { get; set; }
    public List<CombatantView> Targets { get; set; }

    public CombatantView Caster { get; private set; }
    
    public int ModifiedAmount { get; set; }

    public DealDamageGA(int amount, List<CombatantView> targets, CombatantView caster)
    {
        Amount = amount;
        ModifiedAmount = amount; 
        Targets = new(targets);
        Caster = caster;
    }
}
