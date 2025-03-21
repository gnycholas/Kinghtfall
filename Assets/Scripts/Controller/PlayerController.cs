﻿using System.Collections;
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
    [SerializeField] private AnimationClip hitAnimationClip;    // Animação de hit
    [SerializeField] private AnimationClip attackAnimationClip;   // Animação de ataque

    [Header("Configurações de Ataque")]
    [SerializeField] private Transform attackPoint;       // Ponto de origem do ataque
    [SerializeField] private float attackRange = 1.5f;      // Alcance do ataque
    [SerializeField] private LayerMask enemyLayer;          // Layer dos inimigos

    // Variável para controlar a rotação suave de 180° (estado transitório no Controller)
    private bool isTurningAround = false;

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
        // Se estiver bloqueado (morto, hit, atacando, bebendo ou coletando) bloqueia o movimento
        if (playerModel.isDead || playerModel.isHit || playerModel.isDrinking || playerModel.isAttacking || playerModel.isCatching)
        {
            playerView.UpdateAnimations(false, false, false);
            characterController.SimpleMove(Vector3.zero);
            return;
        }

        playerModel.isInjured = (playerModel.currentHealth < 3 && !playerModel.isDead);

        // Inputs
        float vertical = Input.GetAxis("Vertical");    // W/S – avanço/recuo
        float horizontal = Input.GetAxis("Horizontal"); // A/D – rotação ou strafe

        // Reset dos estados de giro
        playerModel.isTurningLeft = false;
        playerModel.isTurningRight = false;
        playerModel.isBacking = false;

        Vector3 moveDirection = Vector3.zero;

        if (vertical > 0.1f)
        {
            // Avanço: rotação livre e movimento para frente
            float rotationSpeed = 150f;
            transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);
            moveDirection = transform.forward * vertical;
        }
        else if (vertical < -0.1f)
        {
            // Se o jogador estiver andando para trás e pressionar Shift, inicia a rotação suave de 180°
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && !isTurningAround)
            {
                StartCoroutine(TurnAroundRoutine());
                moveDirection = Vector3.zero; // Pausa o movimento durante a rotação
            }
            else if (!isTurningAround)
            {
                // Permite rotação normal enquanto se move para trás
                float rotationSpeed = 150f;
                transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);
                playerModel.isBacking = true;
                moveDirection = transform.forward * vertical;
            }
        }
        else
        {
            // Parado: permite rotação para mudar de direção e marca os estados de giro
            float idleRotationSpeed = 100f;
            transform.Rotate(0, horizontal * idleRotationSpeed * Time.deltaTime, 0);
            if (horizontal < -0.1f)
                playerModel.isTurningLeft = true;
            else if (horizontal > 0.1f)
                playerModel.isTurningRight = true;
            moveDirection = transform.forward * vertical; // próximo de zero se vertical está perto de 0
        }

        bool isRunning = false;
        if (vertical > 0.1f)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            playerModel.isWalking = true;
        }
        else if (Mathf.Abs(vertical) > 0.1f)
        {
            playerModel.isWalking = true;
            isRunning = false;
        }
        else
        {
            playerModel.isWalking = false;
            isRunning = false;
        }
        playerModel.isRunning = isRunning;

        float currentSpeed = isRunning ? playerModel.runSpeed : playerModel.walkSpeed;
        characterController.SimpleMove(moveDirection * currentSpeed);

        playerView.UpdateTurning(playerModel.isTurningLeft, playerModel.isTurningRight, playerModel.isBacking);
        playerView.UpdateAnimations(playerModel.isWalking, playerModel.isRunning, playerModel.isInjured);
    }

    // Coroutine para realizar uma rotação suave de 180° ao virar
    private IEnumerator TurnAroundRoutine()
    {
        isTurningAround = true;
        float duration = 0.5f; // duração da rotação (em segundos) – ajuste conforme necessário
        float elapsed = 0f;
        Quaternion initialRotation = transform.rotation;
        // Calcula a rotação alvo: adiciona 180° ao ângulo Y atual
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 180, 0));

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;
        isTurningAround = false;
    }
    #endregion

    #region Combate
    private void HandleCombat()
    {
        // Toggle para a faca (Alpha1) – apenas se houver faca no inventário
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
                ghoul.TakeDamage(1);
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
                playerModel.isPotionEquipped = false;
                playerView.UpdatePotionEquip(false);
            }
            else if (HasPotionInInventory())
            {
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
                playerModel.isKeyEquipped = false;
                playerView.UpdateKeyEquip(false);
            }
            else if (HasKeyInInventory())
            {
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
        float potionAnimDuration = 2.75f;
        yield return new WaitForSeconds(potionAnimDuration);

        playerModel.currentHealth = 5;
        playerModel.isPotionEquipped = false;
        playerModel.isDrinking = false;
        playerView.UpdateDrinking(false);
        playerView.UpdatePotionEquip(false);
        RemovePotionFromInventory();
        Debug.Log("Poção consumida: vida restaurada para 5");
    }

    public bool HasPotionInInventory()
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

    public bool HasKeyInInventory()
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

    public bool HasKnifeInInventory()
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

    // Rotina de coleta: o player rotaciona para o item, ativa o estado de "catching" e atualiza a animação
    public IEnumerator CatchItemRoutine(GameObject item)
    {
        // Calcula a direção até o item ignorando a componente vertical
        Vector3 direction = item.transform.position - transform.position;
        direction.y = 0f;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rotationSpeed = 5f;

        // Enquanto o player não estiver alinhado com o alvo
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            float signedAngle = Vector3.SignedAngle(transform.forward, targetRotation * Vector3.forward, Vector3.up);
            if (signedAngle > 0f)
            {
                playerModel.isTurningRight = true;
                playerModel.isTurningLeft = false;
            }
            else if (signedAngle < 0f)
            {
                playerModel.isTurningLeft = true;
                playerModel.isTurningRight = false;
            }
            else
            {
                playerModel.isTurningLeft = false;
                playerModel.isTurningRight = false;
            }
            playerView.UpdateTurning(playerModel.isTurningLeft, playerModel.isTurningRight, false);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        playerModel.isTurningLeft = false;
        playerModel.isTurningRight = false;
        playerView.UpdateTurning(false, false, false);

        playerModel.isCatching = true;
        playerView.UpdateCatching(true);
        float catchDuration = 1f;
        yield return new WaitForSeconds(catchDuration);
        AddItemToInventory(item);
        playerModel.isCatching = false;
        playerView.UpdateCatching(false);
    }

    public bool AddItemToInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ao inventário.");
            return false;
        }
        if (playerModel.inventory == null)
            playerModel.inventory = new List<GameObject>();
        playerModel.inventory.Add(item);
        item.SetActive(false);
        Debug.Log($"Item '{item.name}' adicionado ao inventário. Total de itens: {playerModel.inventory.Count}");
        return true;
    }
    public void SetCatching(bool state)
    {
        playerModel.isCatching = state;
        playerView.UpdateCatching(state);
    }
    #endregion
}
