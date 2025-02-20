using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [Header("Refer�ncias ao Model e View")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerView playerView;
    [SerializeField] private CharacterController characterController;

    [Header("Refer�ncias a Objetos de Cena")]
    [SerializeField] private GameObject daggerGameObject; // Objeto da faca

    [Header("Refer�ncias a Objetos de Cena")]
    [SerializeField] private GameObject potionGameObject; // Objeto do potion

    [Header("Configura��es de Anima��o")]
    [SerializeField] private AnimationClip hitAnimationClip;    // Anima��o de hit (ao receber dano)
    [SerializeField] private AnimationClip attackAnimationClip; // Anima��o de ataque

    [Header("Configura��es de Ataque")]
    [SerializeField] private Transform attackPoint;           // Ponto de origem do ataque
    [SerializeField] private float attackRange = 1.5f;          // Alcance do ataque
    [SerializeField] private LayerMask enemyLayer;            // Layer dos inimigos

    // Utilizada para orientar o jogador durante a movimenta��o
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
        HandleInventoryInput();
        HandleCombat();
        HandleMovement();
    }

    #region Movimenta��o
    private void HandleMovement()
    {
        if (playerModel.isDead || playerModel.isHit || playerModel.isAttacking || playerModel.isDrinking)
        {
            playerView.UpdateAnimations(false, false, false);
            characterController.SimpleMove(Vector3.zero);
            return;
        }

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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.2f);

        playerView.UpdateAnimations(playerModel.isWalking, playerModel.isRunning, playerModel.isInjured);
    }
    #endregion

    #region Combate
    private void HandleCombat()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerModel.isKnifeEquipped = !playerModel.isKnifeEquipped;
            daggerGameObject.SetActive(playerModel.isKnifeEquipped);
            playerView.UpdateKnifeEquip(playerModel.isKnifeEquipped);
        }

        if (playerModel.isKnifeEquipped && !playerModel.isAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerModel.isAttacking = true;
                playerView.SetAttacking(true);
                playerView.TriggerAttack();
                float attackDuration = (attackAnimationClip != null) ? attackAnimationClip.length : 1.3f;
                StartCoroutine(ResetAttack(attackDuration));
            }
        }
    }

    private IEnumerator ResetAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerModel.isAttacking = false;
        playerView.SetAttacking(false);
    }

    public void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            GhoulPatrolController ghoul = enemy.GetComponent<GhoulPatrolController>();
            if (ghoul != null)
            {
                ghoul.TakeDamage(1); // Ajuste o dano conforme necess�rio
                Debug.Log("ghoul sofreu o dano");
            }
        }
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

    #region Invent�rio e Po��o
    private void HandleInventoryInput()
    {
        // Pressiona Alpha2 para equipar a po��o, se houver no invent�rio
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (HasPotionInInventory())
            {
                playerModel.isPotionEquipped = true;
                potionGameObject.SetActive(playerModel.isKnifeEquipped);
                playerView.UpdatePotionEquip(true);
            }
        }

        // Se a po��o estiver equipada e o jogador clicar com o bot�o esquerdo, consome a po��o
        if (playerModel.isPotionEquipped && Input.GetMouseButtonDown(0))
        {
            // Define que o jogador est� bebendo a po��o (bloqueando outros movimentos)
            playerModel.isDrinking = true;
            playerView.UpdateDrinking(true);
            playerView.TriggerPotionDrink();
            StartCoroutine(ConsumePotionRoutine());
        }
    }

    private IEnumerator ConsumePotionRoutine()
    {
        // Aguarda a dura��o da anima��o de beber a po��o (ajuste conforme necess�rio)
        float potionAnimDuration = 2.75f;
        yield return new WaitForSeconds(potionAnimDuration);

        // Ao final da anima��o:
        playerModel.currentHealth = 5;             // Restaura a vida para 5
        playerModel.isPotionEquipped = false;        // Desativa o estado de po��o equipada
        playerModel.isDrinking = false;              // Libera a movimenta��o (n�o est� mais bebendo)
        playerView.UpdateDrinking(false);
        playerView.UpdatePotionEquip(false);
        RemovePotionFromInventory();
        Debug.Log("Po��o consumida: vida restaurada para 5");
    }

    private bool HasPotionInInventory()
    {
        if (playerModel.inventory == null)
            return false;

        foreach (GameObject item in playerModel.inventory)
        {
            if (item != null && item.CompareTag("Potion"))
                return true;
        }
        return false;
    }

    private void RemovePotionFromInventory()
    {
        if (playerModel.inventory == null)
            return;

        for (int i = 0; i < playerModel.inventory.Count; i++)
        {
            if (playerModel.inventory[i] != null && playerModel.inventory[i].CompareTag("Potion"))
            {
                playerModel.inventory.RemoveAt(i);
                Debug.Log("Po��o removida do invent�rio.");
                break;
            }
        }
    }

    public bool AddItemToInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ao invent�rio.");
            return false;
        }

        if (playerModel.inventory == null)
        {
            playerModel.inventory = new List<GameObject>();
        }

        playerModel.inventory.Add(item);
        item.SetActive(false);
        Debug.Log($"Item '{item.name}' adicionado ao invent�rio. Total de itens: {playerModel.inventory.Count}");
        return true;
    }
}
#endregion
