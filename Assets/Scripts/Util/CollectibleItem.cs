using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class CollectibleItem : MonoBehaviour
{
    public UnityEvent OnInteract;
    [Tooltip("Raio de alcance (em unidades) para visualizar o Gizmo deste item.")]
    public float collectionRadius = 2f;
    private bool _isVisible;
    private bool _isInteracting;
    private PlayerController _playerController; 
    [SerializeField] private AnimatorOverrideController _animatorOverrideController;
    [SerializeField] private string _message;
    [SerializeField] private float _delay;

    private void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
    }

    private void Update()
    {
        var sqrDistance = (_playerController.transform.position - transform.position).sqrMagnitude;
        if (sqrDistance < collectionRadius * collectionRadius && !_isVisible)
        {
            _isVisible = true;
            FindAnyObjectByType<SubtitleGameplayController>().ShowText(_message);
        }
        else if (sqrDistance >= collectionRadius * collectionRadius && _isVisible) 
        {
            _isVisible = false; 
            FindAnyObjectByType<SubtitleGameplayController>().Hidden();
        }

        if (_isVisible && !_isInteracting && Input.GetKeyDown(KeyCode.E))
        {
            _isInteracting = true;
            Interact();
        }
    }

    private async void Interact()
    {
        var time = TimeSpan.FromSeconds(_delay); 
        await Task.Delay(time);
        OnInteract?.Invoke(); 
        FindAnyObjectByType<SubtitleGameplayController>().Hidden(); 
    }
     
}
