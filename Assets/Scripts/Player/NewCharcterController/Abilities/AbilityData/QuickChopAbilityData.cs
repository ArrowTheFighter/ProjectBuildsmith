using System;
using UnityEngine;

[Serializable]
public class QuickChopAbilityData : AbilityData
{
    public float damage;
    public AttackType[] attackTypes;

    public override Type GetAbilityType()
    {
        return typeof(QuickChopAbility);
    }
}
