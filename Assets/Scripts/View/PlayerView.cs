using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Referência ao Animator")]
    [SerializeField] private Animator animator;

    [Header("Referência ao GameObject da Faca (opcional)")]
    [SerializeField] private GameObject knifeGameObject;

    public void UpdateAnimations(bool isWalking, bool isRunning, bool isInjured)
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInjured", isInjured);
    }

    public void TriggerHit()
    {
        animator.SetTrigger("Hit");
        animator.SetBool("isHit", true);
    }

    public void ResetHitAnimation()
    {
        animator.SetBool("isHit", false);
    }

    public void UpdateDeath(bool isDead)
    {
        animator.SetBool("isDead", isDead);
    }

    public void UpdateKnifeEquip(bool isKnifeEquiped)
    {
        animator.SetBool("isKnifeEquiped", isKnifeEquiped);
        if (knifeGameObject != null)
        {
            knifeGameObject.SetActive(isKnifeEquiped);
        }
    }
}
