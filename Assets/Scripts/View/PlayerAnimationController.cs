using System; 
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private bool _canUpdateAnimation = true;
    private readonly int WALK_HASH = Animator.StringToHash("Walk");
    private readonly int WALK_INJURED_HASH = Animator.StringToHash("Walk_Injured");
    private readonly int TURN_HASH = Animator.StringToHash("Turn"); 
    private readonly int WALK_BACK_HASH = Animator.StringToHash("Walking_Back"); 
    private readonly int IDLE_HASH = Animator.StringToHash("Idle");
    private readonly int IDLE_INJURED_HASH = Animator.StringToHash("Idle_Injured"); 
    private readonly int RUN_HASH = Animator.StringToHash("Run");
    private readonly int RUN_INJURED_HASH = Animator.StringToHash("Run_Injured");


    private int _currentClip;


    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.OnUpdateAnimation.AddListener(UpdateAnimations);
        playerController.OnDead.AddListener(Dead);
        GetComponent<InventoryController>().OnWeaponEquip.AddListener(OnChangeWeapon);
        playerController.OnTakeDamage.AddListener(TakeDamage);
    }
    private void OnDisable()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.OnUpdateAnimation.RemoveListener(UpdateAnimations);
        playerController.OnDead.RemoveListener(Dead);
        GetComponent<InventoryController>().OnWeaponEquip.RemoveListener(OnChangeWeapon);
        playerController.OnTakeDamage.RemoveListener(TakeDamage);
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

    private void InjuredMove(UpdateAnimation animation)
    {
        if (animation.VelocityZ > 0)
        {
            var status = animator.GetCurrentAnimatorStateInfo(0);
            if (animation.Run)
            {
                if (_currentClip == RUN_INJURED_HASH)
                    return;
                _currentClip = status.shortNameHash;
                animator.Play("Run_Injured");
            }
            else
            {
                if (_currentClip == WALK_INJURED_HASH)
                    return;
                _currentClip = status.shortNameHash;
                animator.Play("Walk_Injured");
            }
        }
        else if (animation.VelocityZ < 0)
        {
            var status = animator.GetCurrentAnimatorStateInfo(0);
            if (_currentClip == WALK_BACK_HASH)
                return;
            _currentClip = status.shortNameHash;
            animator.Play("Walking_Back");
        }
        else
        {
            if (!Mathf.Approximately(animation.VelocityX, 0))
            {
                var status = animator.GetCurrentAnimatorStateInfo(0);
                if (_currentClip == TURN_HASH)
                    return;
                _currentClip = status.shortNameHash;
                animator.Play("Turn");
            }
            else
            {
                var status = animator.GetCurrentAnimatorStateInfo(0);
                if (_currentClip == IDLE_INJURED_HASH)
                    return;
                _currentClip = status.shortNameHash;
                animator.Play("Idle_Injured");

            }
        }
    }

    private void NormalMove(UpdateAnimation animation)
    {
        if (animation.VelocityZ > 0)
        {
            var status = animator.GetCurrentAnimatorStateInfo(0);
            if (animation.Run)
            {
                if (_currentClip == RUN_HASH)
                    return;
                _currentClip = status.shortNameHash;
                animator.Play("Run");
            }
            else
            {
                if (_currentClip == WALK_HASH)
                    return;
                _currentClip = status.shortNameHash;
                animator.Play("Walk");
            } 
        }
        else if (animation.VelocityZ < 0)
        {
            var status = animator.GetCurrentAnimatorStateInfo(0);
            if (_currentClip == WALK_BACK_HASH)
                return;
            _currentClip = status.shortNameHash;
            animator.Play("Walking_Back"); 
        }
        else
        {
            if (!Mathf.Approximately(animation.VelocityX, 0))
            {
                var status = animator.GetCurrentAnimatorStateInfo(0);
                if (_currentClip == TURN_HASH) 
                    return;
                _currentClip = status.shortNameHash;
                animator.Play("Turn"); 
            }
            else
            {
                var status = animator.GetCurrentAnimatorStateInfo(0);
                if (_currentClip == IDLE_HASH)
                    return; 
                _currentClip = status.shortNameHash;
                animator.Play("Idle");

            }
        }
    }

    public void Dead()
    {
        animator.CrossFade("Die", 0.1f, 0);
        ToggleUpdate(false);
    }
    public void TakeDamage(DamageInfo info)
    {
        animator.CrossFade("Hit",0.1f,0);
        ToggleUpdate(false, animator.GetCurrentAnimatorStateInfo(0).length);
    }
    public void Attack()
    {
        animator.CrossFade("Attack_hit", 0.1f, 0);
        ToggleUpdate(false, animator.GetCurrentAnimatorStateInfo(0).length); 
    }
    public async void ToggleUpdate(bool update, float time)
    {
        _canUpdateAnimation = update;
        await Task.Delay(TimeSpan.FromSeconds(time));
        _canUpdateAnimation = !_canUpdateAnimation;
    }
    public void ToggleUpdate(bool update )
    {
        _canUpdateAnimation = update; 
    }

    public void OnChangeWeapon(Weapon weapon)
    {
        animator.runtimeAnimatorController = weapon.AnimatorOverrideController;
    }
}
public class Weapon : MonoBehaviour
{
    private GameObject _owner;
    [SerializeField] private float _damage;
    [SerializeField] private AnimatorOverrideController _animatorOverrideController;

    public AnimatorOverrideController AnimatorOverrideController { get => _animatorOverrideController;}
}

public struct UpdateAnimation
{
    public float VelocityX;
    public float VelocityZ;
    public bool IsInjured;
    public bool Run;
    public bool Attacking;

    public UpdateAnimation(float x, float z, bool isInjured, bool run,bool attacking) 
    {
        VelocityX = x;
        VelocityZ = z;
        IsInjured = isInjured;
        Run = run;
        Attacking = attacking;
    }
}
