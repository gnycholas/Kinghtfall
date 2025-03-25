using UnityEngine;
using UnityHFSM;

public sealed class EnemyTakeDamage : EnemyState
{
    private Enemy _controller;
    [SerializeField] private float _time;

    private void Awake()
    {
        _controller = GetComponent<Enemy>();
    }

    public override State GetState()
    {
        return new State(onEnter: SetupDamage, onLogic: Logic)
        {
            name = "Enemy TakeDamage",
        };
    }

    private void Logic(State<string, string> state)
    {
        if (state.timer.Elapsed > _time && !_controller.IsDead)
        {
            _controller.RequestStateChange("Enemy Patrol");
        }
    }

    private void SetupDamage(State<string, string> state)
    {
        _controller.Play("Hit");
    }
}
