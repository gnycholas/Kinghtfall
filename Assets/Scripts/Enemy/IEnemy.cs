using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy:IDamageable
{
    public void MoveTo(Vector3 point);
}
public interface IDamageable:IEquatable<GameObject>
{
    public bool IsDead { get; }
    public DamageInfo TakeDamage(Damage damage);
}
public struct Damage
{
    public float Amount;
    public GameObject Origin;

    public Damage(float amount, GameObject origin)
    {
        Amount = amount;
        Origin = origin;
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
