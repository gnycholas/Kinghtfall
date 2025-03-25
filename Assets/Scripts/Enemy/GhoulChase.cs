using System;
using UnityEngine;
using UnityHFSM;

public sealed class GhoulChase : EnemyState
{
    private Enemy _controller;
    private bool _isReady;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _delay;

    private void Awake()
    {
        _controller = GetComponent<Enemy>();
    }

    public override State GetState()
    {
        return new State(onEnter: SetupChase, onLogic: Logic, onExit: Exit)
        {
            name = "Enemy Chase",
        };
    }

    private void Exit(State<string, string> state)
    {
        _controller.Agent.isStopped = true;
        _controller.Agent.speed = _controller.Model.walkSpeed;
        _isReady = false;
    }

    private void Logic(State<string, string> state)
    { 
        if ((state.timer.Elapsed > _delay) && !_isReady)
        { 
            _controller.Agent.isStopped = false;
            _controller.Agent.speed = _controller.Model.runSpeed;
            _controller.Play("Run");
            _isReady = true; 
        }else if( (_controller.LastState == "Enemy Attack" || _controller.LastState == "Enemy Chase") && !_isReady)
        {
            _controller.Agent.isStopped = false;
            _controller.Play("Run");
            _controller.Agent.SetDestination(_controller.Target.position);
            _isReady = true;
        }
        else if(_isReady)
        { 
            float sqrDist = Vector3.SqrMagnitude(_controller.Target.position - transform.position);

            if(!_controller.Agent.pathPending && _controller.Agent.remainingDistance <= _controller.Agent.stoppingDistance)
            {
                if (sqrDist <= (_controller.Model.attackRange * _controller.Model.attackRange))
                {
                    _controller.RequestStateChange("Enemy Attack");
                }
                else
                { 
                    _controller.RequestStateChange("Enemy Chase");
                } 
            }
        } 
    }

    private void SetupChase(State<string, string> state)
    {  
        if (_controller.LastState == "Enemy Patrol" || _controller.LastState == "Enemy Idle")
        { 
            ((Ghoul)_controller).Screaming();
        } 
    }
}
