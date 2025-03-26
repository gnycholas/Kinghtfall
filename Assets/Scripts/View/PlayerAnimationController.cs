using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{ 
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.OnUpdateAnimation.AddListener(UpdateAnimations);
        playerController.OnDead.AddListener(Dead);
    }
    private void OnDisable()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.OnUpdateAnimation.RemoveListener(UpdateAnimations);
        playerController.OnDead.RemoveListener(Dead);

    }
    public void UpdateAnimations(UpdateAnimation animation)
    { 
        if (!animation.IsInjured)
        {
            if(animation.)
            if (animation.VelocityZ > 0)
            {
                animator.Play("Walk");
            }
            else if (animation.VelocityZ < 0)
            {
                animator.Play("Walking_Back");
            }
        }
        if (Mathf.Approximately(animation.VelocityZ, 0))
        {
            animator.Play("Idle");
        }
    }  
    public void Dead()
    {
        animator.Play("Die");
    }
}

public struct UpdateAnimation
{
    public float VelocityX;
    public float VelocityZ;
    public bool IsInjured;
    public bool Run;

    public UpdateAnimation(float x, float z, bool isInjured, bool run) 
    {
        VelocityX = x;
        VelocityZ = z;
        IsInjured = isInjured;
        Run = run;
    }
}
