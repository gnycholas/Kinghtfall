using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityHFSM;

public abstract class Enemy : MonoBehaviour, IEnemy
{
    private StateMachine stateMachine;
    protected Transform target;
    protected string lastState;
    public UnityEvent<string> OnPlayAnimation = new();
    protected PlayerController player;
    private int _currentLife;
    public UnityEvent<DamageInfo> OnTakeDamage;
    public UnityEvent OnDie;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] protected GhoulPatrolModel model;
    [SerializeField] protected string _startState;
    [SerializeField] private Vector2 _lineSighOffset;
    public GhoulPatrolModel Model =>model; 
    public NavMeshAgent Agent { get => agent; }
    public Transform Target { get => target;}

    public string LastState => lastState;

    public bool IsDead => _currentLife <= 0;

    protected virtual void Awake()
    {
        _currentLife = model.maxHealth;
        stateMachine = new StateMachine();
        player = FindAnyObjectByType<PlayerController>();
    }
    protected virtual void Start()
    {
        var states = GetComponents<EnemyState>();
        foreach (var item in states)
        {
            var state = item.GetState();
            stateMachine.AddState(state.name, state);
        }
        stateMachine.SetStartState(_startState);
        stateMachine.Init();
    }
    private void Update()
    { 
        stateMachine.OnLogic(); 
    }
    public virtual bool HasLineOfSightToPlayer()
    {
        if (player.IsDead)
            return false;
        var direction = (player.transform.position - transform.position); 
        if(direction.sqrMagnitude <= (model.detectionRadius * model.detectionRadius))
        {
            var angle = Vector3.Angle(transform.forward, direction.normalized);
            if(angle <= model.fieldOfViewAngle * 0.5f)
            { 
                Vector3 origin = transform.position + new Vector3(0, _lineSighOffset.y, 0);
                Vector3 target = player.transform.position + new Vector3(0, _lineSighOffset.y, 0);
                if (Physics.Linecast(origin, target, out RaycastHit hit, ~_mask))
                    return (hit.collider.gameObject == player.gameObject);
            } 
        }
        return false;
    }
    public DamageInfo TakeDamage(Damage damage)
    {
        if (IsDead)
            return default;
        var realDamage = damage.Amount;
        if (realDamage > 0)
        {
            _currentLife -= Convert.ToInt32(realDamage);
            RequestStateChange("Enemy TakeDamage");
        } 
        
        var info = new DamageInfo() { Critical = false, Damage =realDamage, PercentDamage = 0 };
        OnTakeDamage?.Invoke(info);
        if(_currentLife <= 0)
        {
            Die();
        }
        return info;
    }
    protected void Die()
    {
        Play("Die");
        OnDie?.Invoke();
    }

    public void MoveTo(Vector3 point)
    {
        agent.SetDestination(point);
    }
    public void MoveTo(Vector3 point,float speed)
    {
        MoveTo(point);
        agent.speed = speed;
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void Play(string name)
    {
        OnPlayAnimation?.Invoke(name);  
    }

    public async void RequestStateChange(string v, float delay = 0)
    {
        lastState = stateMachine.ActiveStateName;
        await Task.Delay(TimeSpan.FromSeconds(delay));
        stateMachine.RequestStateChange(v); 
    }

    public bool Equals(GameObject other)
    {
        return other == gameObject;
    }
}
