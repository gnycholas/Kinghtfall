using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy:IDamageable
{
    public void MoveTo(Vector3 point);
}
public interface IDamageable
{
    public DamageInfo TakeDamage(Damage damage);
}
public struct Damage
{
    public int Amount;
}
public struct DamageInfo
{
    public int Damage;
    public float PercentDamage;
    public bool Critical;

    public DamageInfo(int damage, float percentDamage, bool critical)
    {
        Damage = damage;
        PercentDamage = percentDamage;
        Critical = critical;
    }
}
