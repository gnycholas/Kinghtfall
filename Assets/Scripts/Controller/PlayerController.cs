using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour,IDamageable
{
    public UnityEvent<UpdateAnimation> OnUpdateAnimation;
    public UnityEvent OnDead;
    public UnityEvent<DamageInfo> OnTakeDamage; 
    public UnityEvent<ItemCollectibleSO, int> OnCollectItem; 
 
    [Header("Referências ao Model e View")]
    [SerializeField] private PlayerModel playerModel; 
    [SerializeField] private CharacterController characterController;  

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
    private bool _hasWeapon;
    private Vector2 _moveDirection;
    #endregion
    private void Awake()
    {
        if (playerModel == null)
            playerModel = Resources.Load<PlayerModel>("PlayerModel"); 
        if (characterController == null)
            characterController = GetComponent<CharacterController>(); 

        _inputs = new GameInputs();
        _inputs.Gameplay.Enable();
    }

    private void Update()
    { 
        HandleCombat();
        HandleMovement(); 
        OnUpdateAnimation?.Invoke(new UpdateAnimation(_moveDirection.x,
            _moveDirection.y,
            _isInjured,
            _isRun,
            _isAttacking));
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

        _isRun = _inputs.Gameplay.Run.IsPressed(); 

        float currentSpeed = _isRun ? playerModel.runSpeed : playerModel.walkSpeed;
        characterController.SimpleMove(moveDirection * currentSpeed); 
    }
    #endregion

    #region Combate
    private void HandleCombat()
    {
        if (!_isAttacking || !_hasWeapon)
            return;
        if (_inputs.Gameplay.Attack.WasPerformedThisFrame())
        {
            ToggleAttack(true, 1.3f);   
        }
    }

    private async void ToggleAttack(bool active, float time)
    {
        _isAttacking = active;
        await Task.Delay(TimeSpan.FromSeconds(time));
        _isAttacking = !active;
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
    public void AddItemToInventory(ItemCollectibleSO item, int amount)
    {
        OnCollectItem?.Invoke(item, amount); 
        ToggleMove(false, item.Time);
    } 

    public bool Equals(GameObject other)
    {
        return gameObject == other;
    }
    #endregion
} 
