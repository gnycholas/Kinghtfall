using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private GameObject _owner;
    private bool _active;
    [SerializeField] private float _damage;
    [SerializeField] private AnimatorOverrideController _animatorOverrideController;

    public AnimatorOverrideController AnimatorOverrideController { get => _animatorOverrideController;}
     

    private void OnTriggerEnter(Collider other)
    {
        if (!_active)
            return;
        if (other.gameObject == _owner)
            return;
        if(other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(new Damage(_damage, _owner));
        }
    }
    public void ToggleAttack(bool active)
    {
        _active = active;
    }
    public async UniTask ToggleAttack(bool active, float elapsedTime)
    {
        ToggleAttack(active);
        await UniTask.Delay(TimeSpan.FromSeconds(elapsedTime));
        ToggleAttack(!active);
    }
    public void SetupOwner(GameObject owner)
    {
        _owner = owner;
    }
}
