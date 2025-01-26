using UnityEngine;

public class GhoulPatrolView : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    private static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    private static readonly int ScreamTriggerHash = Animator.StringToHash("screamTrigger");

    // Idle
    public void PlayIdleAnimation()
    {
        if (animator == null) return;
        animator.SetBool(IsWalkingHash, false);
        animator.SetBool(IsRunningHash, false);
    }

    // Walk
    public void PlayWalkAnimation()
    {
        if (animator == null) return;
        animator.SetBool(IsWalkingHash, true);
        animator.SetBool(IsRunningHash, false);
    }

    // Run
    public void PlayRunAnimation()
    {
        if (animator == null) return;
        animator.SetBool(IsWalkingHash, false);
        animator.SetBool(IsRunningHash, true);
    }

    // Scream
    public void PlayScreamAnimation()
    {
        if (animator == null) return;
        // Exemplo: um trigger que inicia a animação de scream
        animator.SetTrigger(ScreamTriggerHash);
    }
}
