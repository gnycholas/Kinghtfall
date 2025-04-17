using System; 
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zenject;

public class DoorController : MonoBehaviour,IInteract
{
    public UnityEvent OnOpenDoor;
    private Transform _door;
    [SerializeField] private string _sceneName;
    [SerializeField] private string _spawnName;
    [Inject] private GameplayController _gameplayController;
    [Inject] private SceneTransitionController _sceneTransitionController;
    [SerializeField] private Requirement _requirement;
     
    private void Awake()
    {
        _door = transform.GetChild(0);
    }
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _sceneTransitionController.LoadTransitionScene(0, new SceneLoadParams()
            {
                FadeTime = 0.5f,
                Scene = _sceneName,
                DoorIndex = 0,
            });
        }
    }
    public async Task Execute()
    { 
        if (_requirement.Check(_gameplayController))
        { 
            await Open();
        }
    }

    private async Task Open()
    {
        OnOpenDoor?.Invoke();
        _gameplayController.PlayerController.ToggleMove(false);
        _sceneTransitionController.LoadTransitionScene(0, new SceneLoadParams()
        {
            FadeTime = 0.5f,
            Scene = _sceneName,
            DoorIndex = 0,
        });
    }

    public Transform GetTarget()
    {
        return transform;
    }

    public AnimatorOverrideController GetInteraction()
    {
        return null;
    }
}
