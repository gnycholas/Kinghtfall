using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Referência ao Animator")]
    [SerializeField] private Animator animator;

    [Header("Referência aos Objetos Visuais")]
    [SerializeField] private GameObject knifeGameObject;
    [SerializeField] private GameObject potionGameObject; // Objeto visual da poção (aparece na mão quando equipada)

    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
    private static readonly int HitTriggerHash = Animator.StringToHash("Hit");

    // Parâmetros de movimentação e estados já existentes
    public void UpdateAnimations(bool isWalking, bool isRunning, bool isInjured)
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInjured", isInjured);
    }

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

    public void UpdateKnifeEquip(bool isKnifeEquipped)
    {
        animator.SetBool("isKnifeEquipped", isKnifeEquipped);
        if (knifeGameObject != null)
            knifeGameObject.SetActive(isKnifeEquipped);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(AttackTriggerHash);
    }

    public void SetAttacking(bool attacking)
    {
        animator.SetBool("isAttacking", attacking);
    }

    // Método para atualizar a visualização da poção na mão
    public void UpdatePotionEquip(bool isEquipped)
    {
        // Atualiza o parâmetro do Animator (certifique-se de que o parâmetro está exatamente nomeado "isPotionEquipped")
        animator.SetBool("isPotionEquipped", isEquipped);

        // Ativa ou desativa o objeto visual da poção conforme o estado
        if (potionGameObject != null)
        {
            potionGameObject.SetActive(isEquipped);
        }
    }

    // Novo método para disparar a animação de beber a poção
    public void TriggerPotionDrink()
    {
        animator.SetTrigger("PotionDrink");
    }

    public void UpdateDrinking(bool isDrinking)
    {
        animator.SetBool("isDrinking", isDrinking);
    }

}
