using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Refer�ncia ao Animator")]
    [SerializeField] private Animator animator;

    [Header("Refer�ncia aos Objetos Visuais")]
    [SerializeField] private GameObject knifeGameObject;
    [SerializeField] private GameObject potionGameObject; // Objeto visual da po��o (aparece na m�o quando equipada)

    private static readonly int AttackTriggerHash = Animator.StringToHash("Attack");
    private static readonly int HitTriggerHash = Animator.StringToHash("Hit");

    // Par�metros de movimenta��o e estados j� existentes
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

    // M�todo para atualizar a visualiza��o da po��o na m�o
    public void UpdatePotionEquip(bool isEquipped)
    {
        // Atualiza o par�metro do Animator (certifique-se de que o par�metro est� exatamente nomeado "isPotionEquipped")
        animator.SetBool("isPotionEquipped", isEquipped);

        // Ativa ou desativa o objeto visual da po��o conforme o estado
        if (potionGameObject != null)
        {
            potionGameObject.SetActive(isEquipped);
        }
    }

    // Novo m�todo para disparar a anima��o de beber a po��o
    public void TriggerPotionDrink()
    {
        animator.SetTrigger("PotionDrink");
    }

    public void UpdateDrinking(bool isDrinking)
    {
        animator.SetBool("isDrinking", isDrinking);
    }

}
