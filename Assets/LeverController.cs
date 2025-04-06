using System; 
using System.Threading.Tasks;
using Cysharp.Threading.Tasks; 
using UnityEngine;
using UnityEngine.Events;

public class LeverController : MonoBehaviour,IInteract
{
    public UnityEvent OnActive;
    public UnityEvent OnDesactive;
    private bool _isActive;
    private bool _isOn;
    private bool _transition;
    [SerializeField] private Vector3 _leverDirection;
    [SerializeField] private float _time;

    public void ToggleActive()
    {
        _isActive = !_isActive;
    }
    public async void ToggleOnOrOff(bool action)
    {
        if (_transition || _isActive)
            return;
        _isOn = action;
        _transition = true;
        if (_isOn)
        {
            Active();
        }
        else
        {
            Desative();
        }
        await Transition();
    }
    public async void ToggleAction()
    {
        if (_transition)
            return;
        _isOn = !_isOn;
        _transition = true;
        if (_isOn)
        {
            Active();
        }
        else
        {
            Desative();
        }
        await Transition();
    }
    private async UniTask Transition()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_time));
        _transition = false;
    }
    private void Active()
    {
        _isOn = true;
        transform.GetChild(0).localPosition -= _leverDirection;
        OnActive?.Invoke();
    }
    private void Desative()
    {
        _isOn = false;
        transform.GetChild(0).localPosition += _leverDirection; 
        OnDesactive?.Invoke();
    }

    public Transform GetTarget()
    {
        return transform;
    }

    public async Task Execute()
    {
        ToggleAction();
        await UniTask.CompletedTask;
    }
}
