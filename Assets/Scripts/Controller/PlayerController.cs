using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [Header("Referências ao Model e View")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerView playerView;
    [SerializeField] private CharacterController characterController;

    [Header("Referências a Objetos de Cena")]
    [SerializeField] private GameObject daggerGameObject; // Objeto visual da faca
    [SerializeField] private GameObject potionGameObject; // Objeto visual da poção
    [SerializeField] private GameObject keyGameObject;    // Objeto visual da chave

    [Header("Configurações de Animação")]
    [SerializeField] private AnimationClip hitAnimationClip;    // Animação de hit (ao receber dano)
    [SerializeField] private AnimationClip attackAnimationClip; // Animação de ataque

    [Header("Configurações de Ataque")]
    [SerializeField] private Transform attackPoint;           // Ponto de origem do ataque
    [SerializeField] private float attackRange = 1.5f;          // Alcance do ataque
    [SerializeField] private LayerMask enemyLayer;            // Layer dos inimigos

    // Utilizada para orientar o jogador durante a movimentação
    private Vector3 lastMovementDirection;

    private void Awake()
    {
        if (playerModel == null)
            playerModel = Resources.Load<PlayerModel>("PlayerModel");
        if (playerView == null)
            playerView = GetComponent<PlayerView>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        // Garante que os objetos visuais equipados comecem desativados
        if (daggerGameObject != null)
            daggerGameObject.SetActive(false);
        if (potionGameObject != null)
            potionGameObject.SetActive(false);
        if (keyGameObject != null)
            keyGameObject.SetActive(false);
    }

    private void Update()
    {
        HandleInventoryInput();
        HandleCombat();
        HandleMovement();
    }

    #region Movimentação
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
        // Toggle para a faca com Alpha1 – somente permite se o jogador possuir a faca no inventário
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (HasKnifeInInventory())
            {
                playerModel.isKnifeEquipped = !playerModel.isKnifeEquipped;
                daggerGameObject.SetActive(playerModel.isKnifeEquipped);
                playerView.UpdateKnifeEquip(playerModel.isKnifeEquipped);
            }
            else
            {
                Debug.Log("Você não possui a faca no inventário.");
            }
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
                ghoul.TakeDamage(1); // Ajuste o dano conforme necessário
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

    #region Inventário e Itens (Poção, Chave e Faca)
    private void HandleInventoryInput()
    {
        // Toggle para equipar/desequipar a poção com Alpha2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (playerModel.isPotionEquipped)
            {
                // Desequipa a poção se já estiver equipada
                playerModel.isPotionEquipped = false;
                playerView.UpdatePotionEquip(false);
            }
            else if (HasPotionInInventory())
            {
                // Se a chave ou a faca estiverem equipadas, desequipe-as antes
                if (playerModel.isKeyEquipped)
                {
                    playerModel.isKeyEquipped = false;
                    playerView.UpdateKeyEquip(false);
                }
                if (playerModel.isKnifeEquipped)
                {
                    playerModel.isKnifeEquipped = false;
                    daggerGameObject.SetActive(false);
                    playerView.UpdateKnifeEquip(false);
                }
                playerModel.isPotionEquipped = true;
                playerView.UpdatePotionEquip(true);
            }
        }

        // Se a poção estiver equipada e o jogador clicar com o botão esquerdo, consome a poção
        if (playerModel.isPotionEquipped && Input.GetMouseButtonDown(0))
        {
            playerModel.isDrinking = true;
            playerView.UpdateDrinking(true);
            playerView.TriggerPotionDrink();
            StartCoroutine(ConsumePotionRoutine());
        }

        // Toggle para equipar/desequipar a chave com Alpha3
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (playerModel.isKeyEquipped)
            {
                // Desequipa a chave se já estiver equipada
                playerModel.isKeyEquipped = false;
                playerView.UpdateKeyEquip(false);
            }
            else if (HasKeyInInventory())
            {
                // Se a poção ou a faca estiverem equipadas, desequipe-as antes
                if (playerModel.isPotionEquipped)
                {
                    playerModel.isPotionEquipped = false;
                    playerView.UpdatePotionEquip(false);
                }
                if (playerModel.isKnifeEquipped)
                {
                    playerModel.isKnifeEquipped = false;
                    daggerGameObject.SetActive(false);
                    playerView.UpdateKnifeEquip(false);
                }
                playerModel.isKeyEquipped = true;
                playerView.UpdateKeyEquip(true);
            }
        }
    }

    private IEnumerator ConsumePotionRoutine()
    {
        float potionAnimDuration = 2.75f; // Ajuste conforme a duração real da animação de beber a poção
        yield return new WaitForSeconds(potionAnimDuration);

        // Ao final da animação:
        playerModel.currentHealth = 5;             // Restaura a vida para 5
        playerModel.isPotionEquipped = false;        // Desativa o estado de poção equipada
        playerModel.isDrinking = false;              // Libera a movimentação
        playerView.UpdateDrinking(false);
        playerView.UpdatePotionEquip(false);
        RemovePotionFromInventory();
        Debug.Log("Poção consumida: vida restaurada para 5");
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
                Debug.Log("Poção removida do inventário.");
                break;
            }
        }
    }

    private bool HasKeyInInventory()
    {
        if (playerModel.inventory == null)
            return false;

        foreach (GameObject item in playerModel.inventory)
        {
            if (item != null && item.CompareTag("Key"))
                return true;
        }
        return false;
    }

    private void RemoveKeyFromInventory()
    {
        if (playerModel.inventory == null)
            return;

        for (int i = 0; i < playerModel.inventory.Count; i++)
        {
            if (playerModel.inventory[i] != null && playerModel.inventory[i].CompareTag("Key"))
            {
                playerModel.inventory.RemoveAt(i);
                Debug.Log("Chave removida do inventário.");
                break;
            }
        }
    }

    private bool HasKnifeInInventory()
    {
        if (playerModel.inventory == null)
            return false;

        foreach (GameObject item in playerModel.inventory)
        {
            if (item != null && item.CompareTag("Dagger"))
                return true;
        }
        return false;
    }

    public bool AddItemToInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ao inventário.");
            return false;
        }

        if (playerModel.inventory == null)
        {
            playerModel.inventory = new List<GameObject>();
        }

        playerModel.inventory.Add(item);
        // Desativa o item para que ele desapareça do mundo após a coleta
        item.SetActive(false);
        Debug.Log($"Item '{item.name}' adicionado ao inventário. Total de itens: {playerModel.inventory.Count}");
        return true;
    }
    #endregion
}
