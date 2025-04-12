using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    private GameObject _pausePanel;
    private GameInputs _gameInputs;
    public UnityEvent OnPause;
    public UnityEvent OnResume;
    [SerializeField] private AssetReference _pausePanelRef;

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
            Addressables.ReleaseInstance(_pausePanel);
            _pausePanel = null;
            OnResume?.Invoke();
        }
        else
        {
            _pausePanel = Addressables.InstantiateAsync(_pausePanelRef).WaitForCompletion();
            OnPause?.Invoke();
        }
    }
}

public interface IPause
{
    public void Pause();
    public void Resume();
}