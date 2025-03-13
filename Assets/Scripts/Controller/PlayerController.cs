using System; 
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerController : MonoBehaviour,ITarget
{
    #region Player Stats
    private bool _canMove;
    private bool _canRun;
    private bool _isDead; 
    #endregion
    #region Events
    public UnityEvent<Vector2> OnMove = new();
    public UnityEvent<bool> OnRun = new();
    public Func<Damage,DamageInfo> OnTakeDamage;
    public Func<string,float> OnPlayAnimation;
    public UnityEvent<Item> OnEquipWeapon = new();
    public UnityEvent<Item> OnEquipConsumible = new();
    public UnityEvent<Item> OnCollectItem = new();
    public UnityEvent<Item> OnRemoveItem = new();
    public UnityEvent OnDie = new();
    public UnityEvent OnAttack = new();
    public UnityEvent<bool> OnInjured = new();
    public UnityEvent<AnimatorOverrideController> OnOverrideAnimator = new();
    #endregion
    [Header("Referências ao Model e View")]
    [SerializeField] private PlayerModel playerModel; 
     
   
    private Vector3 lastMovementDirection;

    private void Awake()
    {
        if (playerModel == null)
            playerModel = Resources.Load<PlayerModel>("PlayerModel");  
    }

    private void Update()
    { 
        HandleCombat();
        HandleMovement(); 
    }

    #region Movimentação
    private void HandleMovement()
    {
        // Se estiver bloqueado (morto, hit, atacando, bebendo ou coletando) bloqueia o movimento
        if (_isDead || _canMove)
        { 
            OnMove.Invoke(Vector3.zero);
            return;
        } 
        // Inputs
        float vertical = Input.GetAxis("Vertical");    // W/S – avanço/recuo
        float horizontal = Input.GetAxis("Horizontal"); // A/D – rotação


        OnRun?.Invoke(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));  
        OnMove.Invoke(new Vector2(horizontal,vertical)); 
    }
    #endregion

    #region Combate
    private void HandleCombat()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            OnAttack?.Invoke();
        }
    } 
     
    #endregion

    #region Vida e Dano
    public void TakeDamage(Damage damage)
    {
        if (_isDead)
            return;

        var result = OnTakeDamage.Invoke(damage);

        _isDead = result.Stat == PlayerHealthStat.DEAD;

        if(result.Damage > 0)
        {
            var animationTime = OnPlayAnimation?.Invoke("Hit");
            ToggleMove(false, animationTime);
        }

        if (_isDead)
        {
            OnDie?.Invoke();
            ToggleMove(false);
        } 
    } 
    #endregion
    private void ToggleMove(bool move)
    {
        _canMove = move;
    }
    private async void ToggleMove(bool move, float? time)
    {
        ToggleMove(move); 
        await Task.Delay(TimeSpan.FromSeconds(time??0));
        ToggleMove(!_canMove);
    }
    #region Inventário e Itens (Poção, Chave e Faca)
     

    public bool AddItemToInventory(ItemView item, AnimatorOverrideController animator, float timeMove)
    {
        if (item == null)
        {
            Debug.LogWarning("Tentativa de adicionar item nulo ao inventário.");
            return false;
        }
        ToggleMove(false, timeMove);
        OnOverrideAnimator?.Invoke(animator);
        OnCollectItem?.Invoke(item.Info.CachedToItem());
        return true;
    }

    #endregion

    public bool Equals(ITarget other)
    {
        if (other.GetThisGameObject() == gameObject)
        {
            return true;
        }
        return false;
    }

    public GameObject GetThisGameObject()
    {
        return gameObject;
    }

    internal void EquipItem(Item item)
    {
        OnEquipWeapon?.Invoke(item);
    }
}
public struct Damage
{
    public float Amount; 

    public Damage(float damage)
    {
            Amount = damage;
    }
}
public struct DamageInfo
{
    public float CurrentLife;
    public float Damage;
    public float Percent;
    public PlayerHealthStat Stat;
    public DamageInfo(float currentLife, float percent, PlayerHealthStat stat, float damage)
    {
        Stat = stat;
        Percent = percent;
        Damage = damage;
        CurrentLife = currentLife;
    }
}
public enum PlayerHealthStat
{
    NORMAL,
    INJURED,
    DEAD
}
public record Item
{
    public string Name;
    public int Amount;
    public ItemType Type;
}
public enum ItemType
{
    WEAPON,
    CONSUMIBLE
}
