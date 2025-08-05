using System;
using UnityEngine;

[Serializable]
public class QuickChopAbilityData : AbilityData
{
    public int damage;
    public AttackType[] attackTypes;

    public override Type GetAbilityType()
    {
        return typeof(QuickChopAbility);
    }
}
