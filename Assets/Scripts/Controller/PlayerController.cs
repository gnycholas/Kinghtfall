using UnityEngine;

/// <summary>
/// Responsável por receber inputs, atualizar o modelo (Model)
/// e orquestrar o que deve ser exibido (View).
/// </summary>
[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [Header("Referência ao ScriptableObject Model")]
    [SerializeField] private PlayerModel playerModel;

    [Header("Dependências (View)")]
    private PlayerView playerView;

    [Header("Configurações de Movimento")]
    [SerializeField] private CharacterController characterController;
    // Ou use um Rigidbody, de acordo com a sua preferência.

    private void Awake()
    {
        playerView = GetComponent<PlayerView>();
    }

    private void Update()
    {
        // 1. Capturar Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // 2. Determinar se está andando ou correndo
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude > 0.1f)
        {
            // Se estiver movendo (magnitude > 0.1 para evitar "tremida")
            playerModel.isWalking = true;
            playerModel.isRunning = isShiftPressed;
        }
        else
        {
            // Se não estiver movendo
            playerModel.isWalking = false;
            playerModel.isRunning = false;
        }

        // 3. Calcular velocidade com base no Model
        float currentSpeed = playerModel.isRunning ? playerModel.runSpeed : playerModel.walkSpeed;
        Vector3 velocity = direction * currentSpeed;

        // 4. Aplicar movimento
        // Se estiver usando CharacterController:
        characterController.SimpleMove(velocity);

        // Caso use transform.Translate em vez de CharacterController:
        // transform.Translate(velocity * Time.deltaTime, Space.World);

        // 5. Atualizar rotação do personagem (opcional)
        // Se quiser que o personagem rotacione para a direção do movimento:
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                0.2f
            );
        }

        // 6. Pedir para a View atualizar as animações
        playerView.UpdateAnimations(playerModel.isWalking, playerModel.isRunning);
    }
}
