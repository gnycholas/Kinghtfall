using System;
using UnityEngine;

public class Consumible : MonoBehaviour
{
    [SerializeField] private float _life;
    [SerializeField] private AnimatorOverrideController _animatorOverrideController;
    [SerializeField] private ItemSO _info;

    public AnimatorOverrideController AnimatorOverrideController { get => _animatorOverrideController; }
    public float Life { get => _life;}
    public ItemSO Info { get => _info;}

    public void Consume(PlayerController playerController)
    {
        playerController.RecoverLife(_life);
        Destroy(gameObject, 1.2f);
    }
}
