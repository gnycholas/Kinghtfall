using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public abstract class Enemy : MonoBehaviour, IEnemy
{
    protected StateMachine stateMachine;
    [SerializeField] protected GhoulPatrolView view;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected GhoulPatrolModel model;
    public GhoulPatrolModel Model => model;

    protected void Start()
    {
        var states = GetComponents<EnemyState>();
    }
    public DamageInfo TakeDamage(Damage damage)
    {
        throw new System.NotImplementedException();
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
}

public sealed class Ghoul : Enemy
{

}

public abstract class EnemyState : MonoBehaviour
{
    public abstract State GetState();
}

public sealed class EnemyIdle : EnemyState
{
    private Enemy _controll;
    [SerializeField] private float _time;

    private void Awake()
    {
        _controll = GetComponent<Enemy>();
    }

    public override State GetState()
    {
        return new State() { }
    }
}
