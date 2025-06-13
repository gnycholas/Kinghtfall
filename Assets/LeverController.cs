using System; 
using System.Threading.Tasks;
using Cysharp.Threading.Tasks; 
using UnityEngine;
using UnityEngine.Events;

public class LeverController : MonoBehaviour,IInteract,IEnvironmentState
{
    public UnityEvent OnActive;
    public UnityEvent OnEndInteraction;
    public UnityEvent OnDesactive;
    private bool _isActive;
    private bool _isOn;
    private bool _transition;
    private bool _firstTime = true;
    [SerializeField] private bool _canSwitch;
    [SerializeField] private Transform _leverTransform; 
    [SerializeField] private float _time;
    [SerializeField] private string _name;

    public int Hash => _name.GetHashCode();

    public void ToggleActive()
    {
        _isActive = !_isActive;
        if (_isActive)
        {
            GetComponent<EnviromentStateCheck>().ChangeState(Hash,true);
        }
    }
    public async Task ToggleOnOrOff(bool action)
    {
        if (_transition || _isActive)
            return;
        _isOn = action;
        _transition = true;
        if (_isOn)
        {
            await Active();
        }
        else
        {
            await Desative();
        }
        await Transition();
    }
    public async Task ToggleAction()
    {
        if (!_canSwitch && !_firstTime)
            return;
        if (_firstTime)
        {
            _firstTime = false;
        }
        if (_transition)
            return;
        _isOn = !_isOn;
        _transition = true; 
        OnActive?.Invoke();
        if (_isOn)
        {
            await Active();
        }
        else
        {
            await Desative();
        }
        await Transition();
    }
    private async UniTask Transition()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_time));
        _transition = false;
        OnEndInteraction?.Invoke();
    }
    private async Task Active()
    {
        _isOn = true;
        var finalRotationEuler = new Vector3(50, 0, 0);
        var end = Quaternion.Euler(finalRotationEuler);
        while(_leverTransform.localRotation != end)
        {
            _leverTransform.localRotation = Quaternion.RotateTowards(_leverTransform.localRotation, end, Time.deltaTime * 20);
            await UniTask.NextFrame();
        } 
    }
    private async Task Desative()
    {
        _isOn = false;
        var finalRotationEuler = new Vector3(0, 0, 0);
        var end = Quaternion.Euler(finalRotationEuler);
        while (_leverTransform.localRotation != end)
        {
            _leverTransform.localRotation = Quaternion.RotateTowards(_leverTransform.localRotation, end, Time.deltaTime * 20);
            await UniTask.NextFrame();
        } 
    }

    public Transform GetTarget()
    {
        return transform;
    }

    public async Task Execute()
    {
        await ToggleAction(); 
    }

    public AnimatorOverrideController GetInteraction()
    {
        return null;
    }

    public void ChangeState(bool active)
    {
        _isActive = !active; 
    }
}
