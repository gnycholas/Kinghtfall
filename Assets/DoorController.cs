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
                Scene ="Sei l� bicho"
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
        Vector3 start = _door.localRotation.eulerAngles;
        Vector3 final = start + new Vector3(0, -90, 0);
        float elapsedTime = 0;
        while (true)
        {
            elapsedTime += Time.deltaTime;
            _door.localRotation = Quaternion.Euler(Vector3.Lerp(start, final, elapsedTime));
            await UniTask.NextFrame();
            if(elapsedTime >= 1)
            {
                break;
            }
        }
        await Task.Delay(TimeSpan.FromSeconds(2));
        _gameplayController.PlayerController.ToggleMove(true); 
        OnOpenDoor?.Invoke();
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
