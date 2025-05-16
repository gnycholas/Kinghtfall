using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        var enemy = GetComponent<Enemy>();
        enemy.OnPlayAnimation.AddListener(Play);
    }
    private void OnDisable()
    { 
        var enemy = GetComponent<Enemy>();
        enemy.OnPlayAnimation.RemoveListener(Play);
    }
    public void Play(string name)
    {
        _animator.Play(name);
    }
}