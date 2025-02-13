using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [Header("Referências ao Model e View")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerView playerView;
    [SerializeField] private CharacterController characterController;

    [Header("Referências a Objetos de Cena")]
    [SerializeField] private GameObject daggerGameObject; // Objeto da faca

    [Header("Configurações de Animação")]
    [SerializeField] private AnimationClip hitAnimationClip;    // Animação de hit (ao receber dano)
    [SerializeField] private AnimationClip attackAnimationClip; // Animação de ataque

    [Header("Configurações de Ataque")]
    [SerializeField] private Transform attackPoint;           // Ponto de origem do ataque
    [SerializeField] private float attackRange = 1.5f;          // Alcance do ataque
    [SerializeField] private LayerMask enemyLayer;            // Layer dos inimigos

    // Utilizada para orientar o jogador durante a movimentação (não interfere no ataque)
    private Vector3 lastMovementDirection;

    private void Awake()
    {
        if (playerModel == null)
            playerModel = Resources.Load<PlayerModel>("PlayerModel");

        if (playerView == null)
            playerView = GetComponent<PlayerView>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleCombat();
        HandleMovement();
    }

    #region Movimentação
    private void HandleMovement()
    {
        // Se estiver morto, recebendo hit ou atacando, bloqueia a movimentação
        if (playerModel.isDead || playerModel.isHit || playerModel.isAttacking)
        {
            playerView.UpdateAnimations(false, false, false);
            characterController.SimpleMove(Vector3.zero);
            return;
        }

        // Atualiza o estado de lesão
        playerModel.isInjured = (playerModel.currentHealth < 3 && !playerModel.isDead);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
        lastMovementDirection = direction;

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

        float currentSpeed = (playerModel.isRunning ? playerModel.runSpeed : playerModel.walkSpeed);
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

        // Atualiza as animações de movimento (Idle, Walk, Run, etc.)
        playerView.UpdateAnimations(playerModel.isWalking, playerModel.isRunning, playerModel.isInjured);
    }
    #endregion

    #region Combate
    private void HandleCombat()
    {
        // Alterna o estado de combate (equipar/desequipar a faca) ao pressionar a tecla Alpha1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerModel.isKnifeEquipped = !playerModel.isKnifeEquipped;
            daggerGameObject.SetActive(playerModel.isKnifeEquipped);
            playerView.UpdateKnifeEquip(playerModel.isKnifeEquipped);
        }

        // Se a faca estiver equipada e o personagem não estiver atacando, o clique ativa o ataque
        if (playerModel.isKnifeEquipped && !playerModel.isAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerModel.isAttacking = true;
                playerView.SetAttacking(true);
                //Debug.Log("Triggering Attack");
                playerView.TriggerAttack();

                float attackDuration = (attackAnimationClip != null) ? attackAnimationClip.length : 1.3f;
                StartCoroutine(ResetAttack(attackDuration));
            }
        }
    }

    // Método chamado pela Animation Event no clipe de ataque
    public void PerformAttack()
    {
        // Verifica os inimigos na área de ataque
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        //Debug.Log("Quantidade de inimigos detectados: " + hitEnemies.Length);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Inimigo acertado: " + enemy.name);
        }

    }

    private IEnumerator ResetAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerModel.isAttacking = false;
        playerView.SetAttacking(false);
    }
    #endregion

    #region Vida e Dano
    public void TakeDamage(int damage)
    {
        if (playerModel.isDead)
            return;

        playerModel.ApplyDamage(damage);

        if (playerModel.isDead)
        {
            playerView.UpdateDeath(true);
            playerView.UpdateAnimations(false, false, false);
            Debug.Log("Jogador morreu!");
        }
        else
        {
            playerModel.isHit = true;

            if (playerModel.isKnifeEquipped)
                playerView.TriggerCombatHit();
            else
                playerView.TriggerHit();

            characterController.SimpleMove(Vector3.zero);

            float duration = (hitAnimationClip != null) ? hitAnimationClip.length : playerModel.hitDuration;
            StartCoroutine(ResetHit(duration));
        }
    }

    private IEnumerator ResetHit(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerModel.isHit = false;
        playerView.ResetHitAnimation();
    }
    #endregion

    #region Métodos Públicos
    public PlayerModel GetPlayerModel()
    {
        return playerModel;
    }

    public static PlayerController GetPlayerController(Transform playerTransform)
    {
        return playerTransform.GetComponent<PlayerController>();
    }
    #endregion

    // Opcional: Visualizar o alcance do ataque no Scene View
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
