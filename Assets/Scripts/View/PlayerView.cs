using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Referência ao Animator")]
    [SerializeField] private Animator animator;

    [Header("Referência ao GameObject da Faca (opcional)")]
    [SerializeField] private GameObject knifeGameObject;

    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
    private static readonly int HitTriggerHash = Animator.StringToHash("Hit");

    public void UpdateAnimations(bool isWalking, bool isRunning, bool isInjured)
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInjured", isInjured);
    }

    // Dispara o trigger de hit (o Animator usará a condição isKnifeEquipped para escolher a animação)
    public void TriggerHit()
    {
        animator.SetTrigger(HitTriggerHash);
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

    // Atualiza o estado da faca (e, consequentemente, do combate) no Animator e ativa/desativa o objeto visual
    public void UpdateKnifeEquip(bool isKnifeEquipped)
    {
        animator.SetBool("isKnifeEquipped", isKnifeEquipped);
        if (knifeGameObject != null)
        {
            knifeGameObject.SetActive(isKnifeEquipped);
        }
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(AttackTriggerHash);
    }

    public void SetAttacking(bool attacking)
    {
        animator.SetBool("isAttacking", attacking);
    }
}
