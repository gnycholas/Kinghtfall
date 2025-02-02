using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [Header("Referência ao ScriptableObject Model")]
    [SerializeField] private PlayerModel playerModel;

    [Header("Dependências (View)")]
    private PlayerView playerView;

    [Header("Configurações de Movimento")]
    [SerializeField] private CharacterController characterController;

    [Header("Configuração do Hit")]
    // Referência ao AnimationClip de hit; seu comprimento será usado para definir a duração do estado de hit.
    [SerializeField] private AnimationClip hitAnimationClip;

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
    }

    private void Update()
    {
        // Se o personagem estiver morto, garante que os parâmetros do Animator sejam atualizados para desmarcar isInjured.
        if (playerModel.isDead)
        {
            playerView.UpdateAnimations(false, false, false);
            characterController.SimpleMove(Vector3.zero);
            return;
        }

        // Se o personagem estiver em estado de hit, impede o movimento.
        if (playerModel.isHit)
        {
            characterController.SimpleMove(Vector3.zero);
            return;
        }

        // Atualiza o estado "injured" (se estiver morto, isInjured será false).
        playerModel.isInjured = (!playerModel.isDead && (playerModel.currentHealth < 3));

        // Exemplo: Pressionar a tecla H aciona o hit (ou representa receber dano).
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Reduz a vida e atualiza os estados.
            playerModel.currentHealth = Mathf.Max(playerModel.currentHealth - 1, 0);

            if (playerModel.currentHealth <= 0)
            {
                playerModel.isDead = true;
                playerModel.isInjured = false;
                playerModel.isRunning = false;
                playerModel.isWalking = false;
                playerView.UpdateDeath(true);
                playerView.UpdateAnimations(false, false, false);
            }

            // Aciona o hit: seta o estado e dispara a animação.
            playerModel.isHit = true;
            playerView.TriggerHit();
            characterController.SimpleMove(Vector3.zero);
            // Se o hitAnimationClip estiver atribuído, usa seu comprimento; caso contrário, usa o valor padrão do Model.
            float duration = (hitAnimationClip != null) ? hitAnimationClip.length : playerModel.hitDuration;
            StartCoroutine(ResetHit(duration));
            return; // Interrompe o processamento de movimento neste frame.
        }

        // Processamento normal de movimento:
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        // Define se o personagem está andando ou correndo com base na magnitude do input e na tecla Shift.
        if (direction.magnitude > 0.1f)
        {
            playerModel.isWalking = true;
            playerModel.isRunning = isShiftPressed;
        }
        else
        {
            playerModel.isWalking = false;
            playerModel.isRunning = false;
        }

        // Calcula a velocidade corrente conforme o estado (correndo ou andando).
        float currentSpeed = playerModel.isRunning ? playerModel.runSpeed : playerModel.walkSpeed;
        Vector3 velocity = direction * currentSpeed;

        // Aplica o movimento (SimpleMove já considera Time.deltaTime internamente).
        characterController.SimpleMove(velocity);

        // Atualiza a rotação do personagem para que ele "olhe" na direção do movimento.
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                0.2f
            );
        }

        // Atualiza as animações de locomotion e estados (como isInjured).
        playerView.UpdateAnimations(playerModel.isWalking, playerModel.isRunning, playerModel.isInjured);
    }

    /// <summary>
    /// Coroutine que reseta o estado de hit após a duração definida (baseada no clip de hit).
    /// Enquanto o personagem estiver em hit, o movimento fica bloqueado.
    /// </summary>
    /// <param name="duration">Duração do estado de hit.</param>
    private IEnumerator ResetHit(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerModel.isHit = false;
        playerView.ResetHitAnimation();
    }
}
