using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [Header("Referência ao Model")]
    [SerializeField] private PlayerModel playerModel;

    [Header("Dependências de Movimento")]
    [SerializeField] private CharacterController characterController;

    private PlayerView playerView;

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
    }

    private void Update()
    {
        // Se o jogador estiver morto, bloqueia o movimento e atualiza a animação de morte.
        if (playerModel.isDead)
        {
            playerView.UpdateAnimations(false, false, false);
            characterController.SimpleMove(Vector3.zero);
            return;
        }

        // Se o jogador estiver em estado de hit, impede o movimento.
        if (playerModel.isHit)
        {
            characterController.SimpleMove(Vector3.zero);
            return;
        }

        // Atualiza o estado "injured" com base na vida atual.
        playerModel.isInjured = (playerModel.currentHealth < 3 && !playerModel.isDead);

        // Processamento normal de movimento:
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

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

        float currentSpeed = playerModel.isRunning ? playerModel.runSpeed : playerModel.walkSpeed;
        Vector3 velocity = direction * currentSpeed;
        characterController.SimpleMove(velocity);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                0.2f
            );
        }

        playerView.UpdateAnimations(playerModel.isWalking, playerModel.isRunning, playerModel.isInjured);
    }
}
