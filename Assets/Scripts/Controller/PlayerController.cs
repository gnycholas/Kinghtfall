using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject.ReflectionBaking;

[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour,IDamageable
{
    public UnityEvent<UpdateAnimation> OnUpdateAnimation;
    public UnityEvent OnDead;
    public UnityEvent<DamageInfo> OnTakeDamage;
    [Header("Referências ao Model e View")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private PlayerAnimationController playerView;
    [SerializeField] private CharacterController characterController; 

    [Header("Configurações de Animação")]
    [SerializeField] private AnimationClip hitAnimationClip;    // Animação de hit
    [SerializeField] private AnimationClip attackAnimationClip;   // Animação de ataque

    [Header("Configurações de Ataque")]
    [SerializeField] private Transform attackPoint;       // Ponto de origem do ataque
    [SerializeField] private float attackRange = 1.5f;      // Alcance do ataque
    [SerializeField] private LayerMask enemyLayer;    

    private GameInputs _inputs;
    #region Properties
    public bool IsDead => Mathf.Approximately(_currentLife, 0);
    #endregion
    #region Player Status
    public float _currentLife;
    private bool _canMove;
    private bool _isInjured;
    private bool _isAttacking;
    private bool _isRun;
    private Vector2 _moveDirection;
    #endregion
    private void Awake()
    {
        if (playerModel == null)
            playerModel = Resources.Load<PlayerModel>("PlayerModel");
        if (playerView == null)
            playerView = GetComponent<PlayerAnimationController>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>(); 

        _inputs = new GameInputs();
        _inputs.Gameplay.Enable();
    }

    private void Update()
    {
        HandleInventoryInput();
        HandleCombat();
        HandleMovement();
        Debug.Log(_moveDirection);
        OnUpdateAnimation?.Invoke(new UpdateAnimation(_moveDirection.x, _moveDirection.y,_isInjured));
    }

    #region Movimentação
    private void HandleMovement()
    {
        // Se estiver bloqueado (morto, hit, atacando, bebendo ou coletando) bloqueia o movimento
        if (_canMove)
        { 
            characterController.SimpleMove(Vector3.zero);
            return;
        }

        // Inputs
        _moveDirection = _inputs.Gameplay.Move.ReadValue<Vector2>(); 
        float vertical = _moveDirection.y;    // W/S – avanço/recuo
        float horizontal = _moveDirection.x; // A/D – rotação 
        transform.Rotate(0, horizontal * (Mathf.Approximately(vertical,0)?100:150) * Time.deltaTime, 0); 

        Vector3 moveDirection = transform.forward * vertical;

        _isRun = _inputs.Gameplay.Run.WasPressedThisFrame();  

        float currentSpeed = _isRun ? playerModel.runSpeed : playerModel.walkSpeed;
        characterController.SimpleMove(moveDirection * currentSpeed); 
    }
    #endregion

    #region Combate
    private void HandleCombat()
    {
        if (!_isAttacking)
            return;
        if (_inputs.Gameplay.Attack.WasPerformedThisFrame())
        {
            ToggleAttack(true, 1.3f);  
            PerformAttack(); 
        }
    }

    private async void ToggleAttack(bool active, float time)
    {
        _isAttacking = active;
        await Task.Delay(TimeSpan.FromSeconds(time));
        _isAttacking = !active;
    }

    public void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider target in hitEnemies)
        {
            if(target.TryGetComponent(out IDamageable damageable))
            {
                if(!damageable.Equals(gameObject) && !damageable.IsDead)
                {
                    damageable.TakeDamage(new Damage(1,gameObject));
                    break;
                }
            } 
        }
    }
    #endregion

    #region Vida e Dano
    public DamageInfo TakeDamage(Damage damage)
    {
        if (IsDead)
            return default;

        _currentLife += damage.Amount;
        _isInjured = (_currentLife < playerModel.maxHealth * 0.3f) ? true : false;
        if (IsDead)
        {
            OnDead?.Invoke();
            ToggleMove(false,0);
            return default; 
        }
        ToggleMove(false, playerModel.hitDuration);
        var info = new DamageInfo(damage.Amount, 0, false);
        OnTakeDamage?.Invoke(info);
        return info;
    }
    public async void ToggleMove(bool canMove, float elapsedTime)
    {
        _canMove = canMove;
        await Task.Delay(TimeSpan.FromSeconds(elapsedTime));
        _canMove = !_canMove;
    } 
    #endregion

    #region Inventário e Itens (Poção, Chave e Faca)
    private void HandleInventoryInput()
    {
        
    }     
    public bool AddItemToInventory(GameObject item)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ao inventário.");
            return false;
        } 
        return false;
    } 

    public bool Equals(GameObject other)
    {
        return gameObject == other;
    }
    #endregion
}
