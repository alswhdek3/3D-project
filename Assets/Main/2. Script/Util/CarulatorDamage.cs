using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    None=-1,
    Nomal,
    Critical,
    Miss,
    Max
}
public class CarulatorDamage
{
    public static bool AttackSuccess(float attackRate)
    {
        if (attackRate >= 100f) return true;
        float rate = Random.Range(1f, 101f);
        if(rate<=attackRate)
        {
            return true;
        }
        return false;
    }
    public static int NomalDamage(int attack,int defence)
    {
        int damage = attack - defence;
        return damage;
    }
    public static int criticalDamage(int attack,int criticalAttack)
    {
        int damage = attack + (attack * criticalAttack) / 10;
        return damage;
    }
    public static bool CriticalSuccess(float criticalRate)
    {
        float rate = Random.Range(1, 101f);
        if(rate <= criticalRate)
        {
            return true;
        }
        return false;
    }
}
