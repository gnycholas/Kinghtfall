using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ECM2;
using UnityEngine;
using Zenject;

public class PlayerAnimationController : MonoBehaviour
{
    [Inject] private InventoryController _inventory;
    private bool _canUpdateAnimation = true;
    private bool _isCombat;
    private readonly int WALK_HASH = Animator.StringToHash("Walk");
    private readonly int WALK_INJURED_HASH = Animator.StringToHash("Walk_Injured");
    private readonly int TURN_HASH = Animator.StringToHash("Turn"); 
    private readonly int WALK_BACK_HASH = Animator.StringToHash("Walking_Back"); 
    private readonly int IDLE_HASH = Animator.StringToHash("Idle");
    private readonly int IDLE_INJURED_HASH = Animator.StringToHash("Idle_Injured");
    private readonly int IDLE_COMBAT_HASH = Animator.StringToHash("Attack_Idle");
    private readonly int RUN_HASH = Animator.StringToHash("Run");
    private readonly int RUN_INJURED_HASH = Animator.StringToHash("Run_Injured");
    private readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    private readonly int HIT_HASH = Animator.StringToHash("Hit");
    private readonly int DIE_HASH = Animator.StringToHash("Dying");
    private readonly int HEALING_HASH = Animator.StringToHash("Healing"); 

    private int _currentClip;
    [Inject] private PlayerController _playerController; 
    [Inject] private Animator _animator; 

    private void OnEnable()
    { 
        _playerController.OnUpdateAnimation.AddListener(UpdateAnimations);
        _playerController.OnDead.AddListener(Dead);
        _inventory.OnWeaponUnEquip.AddListener(OnUnEquipWeapon);
        _playerController.OnAttackStart.AddListener(Attack); 
        _inventory.OnWeaponEquip.AddListener(OnChangeWeapon);
        _playerController.OnTakeDamage.AddListener(TakeDamage);
        _playerController.OnConsumeStart.AddListener(Consume);
    }
     

    private void OnDisable()
    {
        _playerController.OnUpdateAnimation.RemoveListener(UpdateAnimations);
        _playerController.OnDead.RemoveListener(Dead);
        _playerController.OnAttackStart.RemoveListener(Attack);
        _inventory.OnWeaponUnEquip.RemoveListener(OnUnEquipWeapon); 
        _inventory.OnWeaponEquip.RemoveListener(OnChangeWeapon);
        _playerController.OnTakeDamage.RemoveListener(TakeDamage);
        _playerController.OnConsumeStart.RemoveListener(Consume);

    }
    public void UpdateAnimations(UpdateAnimation animation)
    {
        if (!_canUpdateAnimation)
            return;
        if (!animation.IsInjured)
        {
            NormalMove(animation);
        }
        else
        {
            InjuredMove(animation);
        } 
    }

    private async void Consume(PlayerController player)
    {
         await AsyncConsume();
        player?.ToggleConsume(false);
    }
    private async Task AsyncConsume()
    {
        ToggleUpdate(false);
        _currentClip = HEALING_HASH;
        _animator.CrossFadeInFixedTime(HEALING_HASH, 0.1f, 0);
        await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == HEALING_HASH);
        var anim = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(TimeSpan.FromSeconds(anim.length));
        ToggleUpdate(true);
    }
    private void InjuredMove(UpdateAnimation animation)
    {
        if (animation.VelocityZ > 0)
        {
            var status = _animator.GetCurrentAnimatorStateInfo(0);
            if (animation.Run)
            {
                if (_currentClip == RUN_INJURED_HASH)
                    return;
                _currentClip = status.shortNameHash;
                _animator.Play("Run_Injured");
            }
            else
            {
                if (_currentClip == WALK_INJURED_HASH)
                    return;
                _currentClip = status.shortNameHash;
                _animator.Play("Walk_Injured");
            }
        }
        else if (animation.VelocityZ < 0)
        {
            var status = _animator.GetCurrentAnimatorStateInfo(0);
            if (_currentClip == WALK_BACK_HASH)
                return;
            _currentClip = status.shortNameHash;
            _animator.Play("Walking_Back");
        }
        else
        {
            if (!Mathf.Approximately(animation.VelocityX, 0))
            {
                var status = _animator.GetCurrentAnimatorStateInfo(0);
                if (_currentClip == TURN_HASH)
                    return;
                _currentClip = status.shortNameHash;
                _animator.Play("Turn");
            }
            else
            {
                var status = _animator.GetCurrentAnimatorStateInfo(0);
                if (_currentClip == IDLE_INJURED_HASH)
                    return;
                _currentClip = status.shortNameHash;
                _animator.Play("Idle_Injured");

            }
        }
    }

    private void NormalMove(UpdateAnimation animation)
    {
        if (animation.VelocityZ > 0)
        { 
            if (animation.Run)
            {
                if (_currentClip == RUN_HASH)
                    return;
                _currentClip = RUN_HASH;
                _animator.CrossFadeInFixedTime(RUN_HASH,0.1f);
            }
            else
            {
                if (_currentClip == WALK_HASH)
                    return;
                _currentClip = WALK_HASH;
                _animator.CrossFadeInFixedTime(WALK_HASH,0.1f);
            } 
        }
        else if (animation.VelocityZ < 0)
        { 
            if (_currentClip == WALK_BACK_HASH)
                return;
            _currentClip = WALK_BACK_HASH;
            _animator.CrossFadeInFixedTime(WALK_BACK_HASH,0.1f); 
        }
        else
        {
            if (!Mathf.Approximately(animation.VelocityX, 0))
            { 
                if (_currentClip == TURN_HASH) 
                    return;
                _currentClip =TURN_HASH;
                _animator.CrossFadeInFixedTime(TURN_HASH, 0.1f); 
            }
            else
            {
                if (_isCombat)
                {
                     
                    if (_currentClip == IDLE_COMBAT_HASH)
                        return;
                    _currentClip = IDLE_COMBAT_HASH;
                    _animator.CrossFadeInFixedTime(IDLE_COMBAT_HASH,0.1f);
                }
                else
                {  
                    if (_currentClip == IDLE_HASH)
                        return;
                    _currentClip = IDLE_HASH;
                    _animator.CrossFadeInFixedTime(IDLE_HASH,0.1f);
                }  
            }
        }
    }

    public void Dead()
    {
        ToggleUpdate(false);
        _currentClip = DIE_HASH;
        _animator.CrossFadeInFixedTime(DIE_HASH, 0.1f, 0);  
    }
    private async UniTaskVoid AsyncTakeDamage(DamageInfo info)
    {
        ToggleUpdate(false); 
        _currentClip = HIT_HASH;
        _animator.CrossFadeInFixedTime(HIT_HASH,0.1f,0);
        await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == ATTACK_HASH);
        var anim = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(TimeSpan.FromSeconds(anim.length));
        ToggleUpdate(true); 
    }
    private void TakeDamage(DamageInfo info)
    {
        AsyncTakeDamage(info).Forget();
    }

    private async UniTask AsyncAttack()
    {
        ToggleUpdate(false);
        _currentClip = ATTACK_HASH;
        _animator.CrossFadeInFixedTime(ATTACK_HASH,0.1f);
        await UniTask.WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == ATTACK_HASH);
        var anim = _animator.GetCurrentAnimatorStateInfo(0);
        await UniTask.Delay(TimeSpan.FromSeconds(anim.length));
        ToggleUpdate(true); 
    }
    private async void Attack(PlayerController callback)
    {
        await AsyncAttack();
        callback?.ToggleAttack(false);
    }
    public async UniTaskVoid ToggleUpdate(bool update, float time)
    {
        _canUpdateAnimation = update;
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        _canUpdateAnimation = !_canUpdateAnimation;
    }
    public void ToggleUpdate(bool update )
    {
        _canUpdateAnimation = update; 
    }

    public void OnChangeWeapon(Weapon weapon)
    {
        _isCombat = true;
        _animator.runtimeAnimatorController = weapon.AnimatorOverrideController;
    }
    public void OnUnEquipWeapon()
    {
        _isCombat &= false;
    }
}

public struct UpdateAnimation
{
    public float VelocityX;
    public float VelocityZ;
    public bool IsInjured;
    public bool Run;
    public bool Attacking; 

    public UpdateAnimation(float x, float z,
        bool isInjured,
        bool run,
        bool attacking ) 
    {
        VelocityX = x;
        VelocityZ = z;
        IsInjured = isInjured;
        Run = run;
        Attacking = attacking; 
    }
}
