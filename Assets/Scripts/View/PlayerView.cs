using UnityEngine;

/// <summary>
/// Responsável pela parte visual do personagem (Animator, GameObject 3D, etc.).
/// </summary>
public class PlayerView : MonoBehaviour
{
    [Header("Referência ao Animator")]
    [SerializeField] private Animator animator;

    // Parâmetros do Animator
    private const string ANIM_IS_WALKING = "isWalking";
    private const string ANIM_IS_RUNNING = "isRunning";

    /// <summary>
    /// Atualiza parâmetros de animação
    /// com base nos valores passados.
    /// </summary>
    public void UpdateAnimations(bool isWalking, bool isRunning)
    {
        animator.SetBool(ANIM_IS_WALKING, isWalking);
        animator.SetBool(ANIM_IS_RUNNING, isRunning);
    }

    // Se quiser controlar rotação do personagem, 
    // ou eventos de animação (Animation Events), 
    // você pode colocar aqui também.
}
