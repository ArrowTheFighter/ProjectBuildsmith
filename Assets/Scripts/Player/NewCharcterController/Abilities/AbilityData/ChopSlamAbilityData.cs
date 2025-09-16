using System;
using UnityEngine;

[Serializable]
public class ChopSlamAbilityData : AbilityData
{
    public float damage;
    public AttackType[] attackTypes;

    public override Type GetAbilityType()
    {
        return typeof(ChopSlamAbility);
    }
}
