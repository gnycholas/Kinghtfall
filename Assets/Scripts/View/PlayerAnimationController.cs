using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private bool _isRun;
    [SerializeField] private Animator _animator;  

    private static readonly int _attackTriggerHash = Animator.StringToHash("Attack");
    private static readonly int _hitTriggerHash = Animator.StringToHash("Hit");
    private static readonly int _walkTriggerHash = Animator.StringToHash("Walk");
    private static readonly int _dieTriggerHash = Animator.StringToHash("Die");
    private static readonly int _runTriggerHash = Animator.StringToHash("Run");
    private static readonly int _idleTriggerHash = Animator.StringToHash("Idle");
    private static readonly int _walkBackTriggerHash = Animator.StringToHash("Walk_Back");
    private static readonly int _injuredWalkTriggerHash = Animator.StringToHash("Injured_walk");
    private static readonly int _injuredIdleTriggerHash = Animator.StringToHash("Injured_walk");
    private static readonly int _turnTriggerHash = Animator.StringToHash("Turn");

    private void OnEnable()
    {
        GetComponent<PlayerController>().OnRun.AddListener(Run);
        GetComponent<PlayerController>().OnMove.AddListener(Move);
        GetComponent<PlayerController>().OnDie.AddListener(Die);
        GetComponent<PlayerController>().OnPlayAnimation += Play;
    }
    private void OnDisable()
    {
        GetComponent<PlayerController>().OnRun.RemoveListener(Run);
        GetComponent<PlayerController>().OnMove.RemoveListener(Move);
        GetComponent<PlayerController>().OnDie.RemoveListener(Die);
        GetComponent<PlayerController>().OnPlayAnimation -= Play;
    }
    private void Move(Vector2 arg0)
    {
        var currentState = _animator.GetCurrentAnimatorStateInfo(0);
        VerticalMove(arg0, currentState);
        HorizontalMove(arg0, currentState);
        if (Mathf.Approximately(arg0.sqrMagnitude, 0) && currentState.shortNameHash != _idleTriggerHash)
        {
            Play("Idle");
        }
    }

    private void HorizontalMove(Vector2 arg0, AnimatorStateInfo currentState)
    {
        if (!Mathf.Approximately(arg0.y, 0))
            return;
        if (arg0.x != 0)
        {
            if (currentState.shortNameHash != _turnTriggerHash)
            {
                Play("Turn");
            } 
        }
    }

    private void VerticalMove(Vector2 arg0, AnimatorStateInfo currentState)
    {
        if (arg0.y != 0)
        { 
            if(arg0.y > 0 && _isRun && currentState.shortNameHash != _runTriggerHash)
            {
                Play("Run");
            }
            else if (arg0.y > 0 && !_isRun && currentState.shortNameHash != _walkTriggerHash)
            {
                Play("Walk"); 
            }
            else if (arg0.y < 0 && currentState.shortNameHash != _walkBackTriggerHash)
            {
                Play("Walk_Back");
            }
        } 
    }

    private void Run(bool arg0)
    {
        _isRun = arg0;
    }

    

    private void Die()
    {
        Play("Die");
    }

    public float Play(string name)
    { 
        var animation = name switch
        {
            "Walk" => _walkTriggerHash,
            "Run" => _runTriggerHash,
            "Walk_Back" => _walkBackTriggerHash,
            "Idle" => _idleTriggerHash,
            "Turn" => _turnTriggerHash,
            "Injured_walk" => _injuredWalkTriggerHash,
            "Injured_idle" => _injuredIdleTriggerHash, 
            "Die" =>_dieTriggerHash
        };
        _animator.Play(animation);

        float time = 0;

        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                time = clip.length;
            }
        }
        return time;
    }      
}
