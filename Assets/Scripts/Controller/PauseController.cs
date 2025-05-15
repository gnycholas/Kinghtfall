using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zenject;

public class PauseController : MonoBehaviour
{
    private GameObject _pausePanel;
    private GameInputs _gameInputs;
    public UnityEvent OnPause;
    public UnityEvent OnResume;
    [Inject] private IUIFactory _uiFactory;  

    private void Start()
    {
        _gameInputs = new GameInputs();
        _gameInputs.Enable();
        _gameInputs.Gameplay.Pause.started += Pause;
    }
    private void OnDisable()
    {
        _gameInputs.Dispose();
        _gameInputs = null;
    }
    private void Pause(InputAction.CallbackContext context)
    {
        if(_pausePanel != null)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        _pausePanel = _uiFactory.Create("Pause");
        OnPause?.Invoke();
    }

    public void Resume()
    {
        Addressables.ReleaseInstance(_pausePanel);
        _pausePanel = null;
        OnResume?.Invoke();
    }
}

public interface IPause
{
    public void Pause();
    public void Resume();
}