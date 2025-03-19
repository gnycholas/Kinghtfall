using UnityEngine;
using UnityHFSM;

public sealed class EnemyAttack : EnemyState
{
    private Enemy _controller;
    private bool _isAttacking;
    [SerializeField] private float _delay;
    private void Awake()
    {
        _controller = GetComponent<Enemy>();
    }

    public override State GetState()
    {
        return new State(onEnter: SetupAttack, onLogic: Logic)
        {
            name = "Enemy Attack",
        };
    }

    private void Logic(State<string, string> state)
    {
         if(state.timer.Elapsed > _delay && !_isAttacking)
        {
            _isAttacking = true;
            if (Vector3.SqrMagnitude(_controller.Target.position - transform.position) <= _controller.Model.attackRange * _controller.Model.attackRange)
            {
                if(_controller.Target.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(new Damage(_controller.Model.attackDamage));
                } 
            } 
        }
    }

    private void SetupAttack(State<string, string> state)
    {
        _controller.Play("Attack");
    }
}
