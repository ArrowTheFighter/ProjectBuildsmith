using System;
using UnityEngine;

[Serializable]
public class BoomerangAbilityData : AbilityData
{
    public int damage;
    public AttackType[] attackTypes;

    public override Type GetAbilityType()
    {
        return typeof(BoomerangThrowAbility);
    }
}
