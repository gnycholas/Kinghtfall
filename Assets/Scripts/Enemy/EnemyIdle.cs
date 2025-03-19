using System;
using UnityEngine;
using UnityHFSM;

public sealed class EnemyIdle : EnemyState
{
    private Enemy _controller; 
    [SerializeField] private float _time;

    private void Awake()
    {
        _controller = GetComponent<Enemy>();
    }

    public override State GetState()
    {
        return new State(onEnter:SetupIdle,onLogic:Logic) 
        {
            name = "Enemy Idle",
        };
    }

    private void Logic(State<string, string> state)
    {
        if(state.timer.Elapsed > _time)
        {
            _controller.RequestStateChange("Enemy Patrol");
        }
    }

    private void SetupIdle(State<string, string> state)
    {
        _controller.Play("Idle"); 
    }
}
