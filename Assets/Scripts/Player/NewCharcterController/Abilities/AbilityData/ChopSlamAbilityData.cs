using System;
using UnityEngine;

[Serializable]
public class ChopSlamAbilityData : AbilityData
{
    public int damage;
    public AttackType[] attackTypes;

    public override Type GetAbilityType()
    {
        return typeof(ChopSlamAbility);
    }
}
