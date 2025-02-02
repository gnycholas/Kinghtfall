using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Referência ao Animator")]
    [SerializeField] private Animator animator;

    [Header("Referência ao GameObject da Faca (opcional)")]
    [SerializeField] private GameObject knifeGameObject;

    /// <summary>
    /// Atualiza os parâmetros de locomotion e estado de injúria.
    /// </summary>
    /// <param name="isWalking">Verdadeiro se estiver andando.</param>
    /// <param name="isRunning">Verdadeiro se estiver correndo.</param>
    /// <param name="isInjured">Verdadeiro se o jogador estiver ferido.</param>
    public void UpdateAnimations(bool isWalking, bool isRunning, bool isInjured)
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isInjured", isInjured);
    }

    /// <summary>
    /// Dispara o trigger para a animação de hit.
    /// </summary>
    public void TriggerHit()
    {
        animator.SetTrigger("Hit");
        animator.SetBool("isHit", true);
    }
    public void ResetHitAnimation()
    {
        animator.SetBool("isHit", false);
    }

    /// <summary>
    /// Atualiza o parâmetro de morte no Animator.
    /// </summary>
    /// <param name="isDead">Verdadeiro se o personagem estiver morto.</param>
    public void UpdateDeath(bool isDead)
    {
        animator.SetBool("isDead", isDead);
    }

    /// <summary>
    /// Atualiza o estado de equipamento da faca.
    /// </summary>
    /// <param name="isKnifeEquiped">Verdadeiro se a faca estiver equipada.</param>
    public void UpdateKnifeEquip(bool isKnifeEquiped)
    {
        animator.SetBool("isKnifeEquiped", isKnifeEquiped);
        if (knifeGameObject != null)
        {
            knifeGameObject.SetActive(isKnifeEquiped);
        }
    }
}
