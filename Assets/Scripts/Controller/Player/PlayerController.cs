using System; 
using System.Threading.Tasks;
using ECM2;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour,IDamageable
{
    public UnityEvent<UpdateAnimation> OnUpdateAnimation;
    public UnityEvent OnDead;
    public UnityEvent<string> OnPlayAudio; 
    public UnityEvent<PlayerController> OnAttackStart;
    public UnityEvent<PlayerController> OnConsumeStart; 
    public UnityEvent<DamageInfo> OnTakeDamage; 
    public UnityEvent<ItemSO, int> OnCollectItem;
    public Weapon CurrentWeapon;
    public Consumible CurrentItem;
 
    [Header("Referências ao Model e View")] 
    [SerializeField] private Transform _weaponSlot;
    [SerializeField] private Transform _consumibleSlot;
    [SerializeField] private PlayerModel playerModel;  

    [Inject] private GameInputs _inputs;
    [Inject] private Character _character;
    #region Properties
    public bool IsDead => Mathf.Approximately(_currentLife, 0);
    #endregion
    #region Player Status
    public float _currentLife;
    private bool _canMove = true;
    private bool _isInjured;
    private bool _isAttacking;
    private bool _isRun;
    private bool _hasWeapon;
    private bool _hasConsumible;
    private bool _isConsuming;
    private Vector2 _moveDirection;
    #endregion
    private void Awake()
    {
        if (playerModel == null)
            playerModel = Resources.Load<PlayerModel>("PlayerModel");
        _currentLife = playerModel.maxHealth;
        _inputs.Gameplay.Interact.started += ctx => Interact();
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
        if (!_canMove)
        { 
            _character.SetMovementDirection(Vector3.zero);
            return;
        }

        // Inputs
        _moveDirection = _inputs.Gameplay.Move.ReadValue<Vector2>(); 
        float vertical = _moveDirection.y;    // W/S – avanço/recuo
        float horizontal = _moveDirection.x; // A/D – rotação 
        _character.AddYawInput(horizontal * _character.rotationRate * Time.deltaTime); 

        Vector3 moveDirection = transform.forward * vertical;

        _isRun = _inputs.Gameplay.Run.IsPressed(); 

        float currentSpeed = _isRun ? playerModel.runSpeed : playerModel.walkSpeed;
        if(Mathf.Abs(vertical) > 0)
        {
            if (_isRun)
            {
                OnPlayAudio?.Invoke("Run");
            }
            else
            {
                OnPlayAudio?.Invoke("Walk");
            }
        }
        _character.SetMovementDirection(transform.forward * vertical * currentSpeed); 
    }
    #endregion

    #region Combate
    private void HandleCombat()
    {
        if (_isAttacking || _isConsuming)
            return;
        if (_inputs.Gameplay.Attack.WasPerformedThisFrame() && _hasWeapon)
        {
            ToggleAttack(true);
            OnAttackStart?.Invoke(this);
        }
        if (_inputs.Gameplay.UseItem.WasPerformedThisFrame() && _hasConsumible)
        {
            ToggleConsume(true);
            OnConsumeStart?.Invoke(this);
        }
    }
    public void ToggleConsume(bool active)
    {
        _isConsuming = active;
        ToggleMove(!active);
        if (_isConsuming)
        { 
            _hasConsumible = false;
            CurrentItem?.Consume(this);
        } 
    }
    public void OnEquipWeapon(Weapon weapon)
    {
        CurrentWeapon = weapon;
        if (_hasWeapon)
        {
            Destroy(_weaponSlot.GetChild(0).gameObject);
        }
        else
        { 
            _hasWeapon = true;
            weapon.transform.SetParent(_weaponSlot, false);
            weapon.SetupOwner(gameObject);
        } 
    }
    public void OnUnEquipWeapon()
    {
        CurrentWeapon = null; 
        _hasWeapon = false; 
        if (_weaponSlot.childCount == 0)
            return;
        Destroy(_weaponSlot.GetChild(0).gameObject);
    }
    public void OnEquipItem(Consumible consumible)
    {
        CurrentItem = consumible;
        if (_hasConsumible)
        {
            Destroy(_consumibleSlot.GetChild(0).gameObject);
        }
        else
        {
            _hasConsumible = true;
            consumible.transform.SetParent(_consumibleSlot, false); 
        }
    }
    public void OnUnEquipItem()
    {
        CurrentItem = null;
        _hasConsumible = false;
        if (_consumibleSlot.childCount == 0)
            return;
        Destroy(_consumibleSlot.GetChild(0).gameObject);
    }

    private async void ToggleAttack(bool active, float time)
    {
        ToggleAttack(active);
        await Task.Delay(TimeSpan.FromSeconds(time));
        ToggleAttack(!active);
    }
    public void ToggleAttack(bool active)
    { 
        CurrentWeapon?.ToggleAttack(active);
        ToggleMove(!active);
        _isAttacking = active; 
    }
    #endregion

    #region Vida e Dano
    public DamageInfo TakeDamage(Damage damage)
    {
        if (IsDead)
            return default;

        _currentLife -= damage.Amount;
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
    public void ToggleMove(bool canMove)
    {
        _canMove = canMove; 
    }
    #endregion

    #region Inventário e Itens (Poção, Chave e Faca) 
    public void AddItemToInventory(ItemSO item, int amount)
    {
        OnCollectItem?.Invoke(item, amount);  
    } 
    public void RecoverLife(float life)
    {
        _currentLife += life;
    }

    public bool Equals(GameObject other)
    {
        return gameObject == other;
    }

    public async void Interact()
    {
        var colliders = Physics.OverlapSphere(transform.position, 2);
        foreach (var item in colliders)
        {
            if(item.TryGetComponent(out IInteract interact))
            {
                if(interact.GetTarget() != null)
                { 
                    var position = interact.GetTarget().position;
                    position.y = transform.position.y;
                    transform.LookAt(position);
                } 
                await interact.Execute();
            }
        }
    }
    #endregion
} 
