using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;


    public void Play(string name)
    {
        _animator.Play(name);
    }
}