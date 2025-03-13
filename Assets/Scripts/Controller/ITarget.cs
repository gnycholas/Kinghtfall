using System;
using UnityEngine;

public interface ITarget:IEquatable<ITarget>
{
    public void TakeDamage(Damage damage);

    public GameObject GetThisGameObject();
}
