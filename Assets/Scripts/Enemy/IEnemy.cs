using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy:IDamageable
{
    public void MoveTo(Vector3 point);
}
public interface IDamageable
{
    public bool IsDead { get; }
    public DamageInfo TakeDamage(Damage damage);
}
public struct Damage
{
    public float Amount;

    public Damage(float amount)
    {
        Amount = amount;
    }
}
public struct DamageInfo
{
    public float Damage;
    public float PercentDamage;
    public bool Critical;

    public DamageInfo(float damage, float percentDamage, bool critical)
    {
        Damage = damage;
        PercentDamage = percentDamage;
        Critical = critical;
    }
}
