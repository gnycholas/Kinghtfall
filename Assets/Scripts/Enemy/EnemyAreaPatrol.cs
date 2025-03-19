using System;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

public sealed class EnemyAreaPatrol : EnemyState
{
    private Enemy _controller;
    [SerializeField] private float _minRange;
    [SerializeField] private float _maxRange;
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private Transform _center;

    private void Awake()
    {
        _controller = GetComponent<Enemy>();
    }
    public override State GetState()
    {
        return new State(onEnter: SetupPatrol,onLogic: Logic)
        {
            name = "Enemy Patrol",
        };
    }

    private void Logic(State<string, string> state)
    {
        if(!_controller.Agent.pathPending && _controller.Agent.remainingDistance <= _controller.Agent.stoppingDistance)
        {
            _controller.RequestStateChange("Enemy Idle");
        }
    }

    private void SetupPatrol(State<string, string> state)
    {
        _controller.Play("Walk");
        float randomRadius = UnityEngine.Random.Range(_minRange, _maxRange);
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * randomRadius;
        randomDir += _center.position;
        randomDir.y = _center.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, _maxRange, NavMesh.AllAreas))
            _controller.MoveTo(hit.position,_patrolSpeed);
        else
            _controller.MoveTo(_center.position,_patrolSpeed);
    }
}
