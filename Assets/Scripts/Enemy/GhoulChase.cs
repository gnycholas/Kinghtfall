using System;
using UnityEngine;
using UnityHFSM;

public sealed class GhoulChase : EnemyState
{
    private Enemy _controller;
    private bool _startedMove;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _delay;

    private void Awake()
    {
        _controller = GetComponent<Enemy>();
    }

    public override State GetState()
    {
        return new State(onEnter: SetupIdle, onLogic: Logic, onExit: Exit)
        {
            name = "Enemy Chase",
        };
    }

    private void Exit(State<string, string> state)
    {
        _controller.Agent.isStopped = false;
    }

    private void Logic(State<string, string> state)
    {
        if (state.timer.Elapsed > _delay && !_startedMove)
        {
            _startedMove = true; 
            _controller.Agent.SetDestination(_controller.Target.position);
            _controller.Agent.speed = _chaseSpeed;
            _controller.Agent.isStopped = false;
            _controller.Play("Run");
        } else if (_startedMove)
        {
            float sqrDist = Vector3.SqrMagnitude(_controller.Target.position - transform.position);
            if (sqrDist <= _controller.Model.attackRange * _controller.Model.attackRange)
            {
                _controller.RequestStateChange("Enemy Attack");
            } 
        }
    }

    private void SetupIdle(State<string, string> state)
    { 
        if (_controller.Target == null)
        { 
            ((Ghoul)_controller).Screaming();
        } 
    }
}
