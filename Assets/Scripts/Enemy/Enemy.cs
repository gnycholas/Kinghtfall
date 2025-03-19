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
    public UnityEvent<string> OnPlayAnimation = new();
    protected Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] protected GhoulPatrolModel model;
    [SerializeField] protected string _startState;
    [SerializeField] private Vector2 _lineSighOffset;
    public GhoulPatrolModel Model => model; 
    public NavMeshAgent Agent { get => agent; }
    public Transform Target { get => target;}

    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
        player = FindAnyObjectByType<PlayerController>().transform;
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
        if (HasLineOfSightToPlayer() && stateMachine.ActiveState.name != "Enemy Chase")
        {
            float sqrDist = Vector3.SqrMagnitude(player.position - transform.position); 
            if(sqrDist < model.detectionRadius * model.detectionRadius)
            {
                Vector3 dirToPlayer = (player.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToPlayer);
                if (angle > model.fieldOfViewAngle * 0.5f)
                {
                    stateMachine.RequestStateChange("Enemy Chase");
                }
            }
        }
    }
    private bool HasLineOfSightToPlayer()
    {
        Vector3 origin = transform.position + new Vector3(0, _lineSighOffset.y,0);
        Vector3 target = player.position + new Vector3(0, _lineSighOffset.y,0);
        if (Physics.Linecast(origin, target, out RaycastHit hit))
            return (hit.transform == player);
        return true;
    }
    public DamageInfo TakeDamage(Damage damage)
    {
        var realDamage = damage.Amount;
        if (realDamage > 0)
        { 
            stateMachine.RequestStateChange("Enemy TakeDamage");
        }
        return new DamageInfo() { Critical = false, Damage =realDamage, PercentDamage = 0 };
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
        await Task.Delay(TimeSpan.FromSeconds(delay));
        stateMachine.RequestStateChange(v);
    }
}
