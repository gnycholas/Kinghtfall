using UnityEngine;

public class GhoulPatrolView : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int ScreamTriggerHash = Animator.StringToHash("screamTrigger");
    private static readonly int AttackTriggerHash = Animator.StringToHash("attackTrigger");
    private static readonly int HitTriggerHash = Animator.StringToHash("hitTrigger");
    private static readonly int DieTriggerHash = Animator.StringToHash("dieTrigger");

    public void PlayIdleAnimation()
    {
        if (!animator) return;
        animator.SetBool(IsWalkingHash, false);
        animator.SetBool(IsRunningHash, false);
    }

    public void PlayWalkAnimation()
    {
        if (!animator) return;
        animator.SetBool(IsWalkingHash, true);
        animator.SetBool(IsRunningHash, false);
    }

    public void PlayRunAnimation()
    {
        if (!animator) return;
        animator.SetBool(IsWalkingHash, false);
        animator.SetBool(IsRunningHash, true);
    }

    public void PlayScreamAnimation()
    {
        if (!animator) return;
        animator.SetTrigger(ScreamTriggerHash);
    }

    public void PlayAttackAnimation()
    {
        if (!animator) return;
        animator.SetTrigger(AttackTriggerHash);
    }

    public void TriggerHit()
    {
        if (!animator) return;
        animator.SetTrigger(HitTriggerHash);
        animator.SetBool("isHit", true);
    }

    public void ResetHitAnimation()
    {
        if (!animator) return;
        animator.SetBool("isHit", false);
    }

    public void TriggerDie()
    {
        if (!animator) return;
        animator.SetTrigger(DieTriggerHash);
    }
}
