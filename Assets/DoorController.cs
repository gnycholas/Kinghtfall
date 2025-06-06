using System.Threading.Tasks; 
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
    [Inject] private FadeController _fadeController;
    [SerializeField] private Requirement _requirement;
    [SerializeField] private SpawnPointModel _spawnPoint;

    private void Awake()
    {
        _door = transform.GetChild(0);
    } 
    public async Task Execute()
    { 
        if (_requirement == null || _requirement.Check(_gameplayController))
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
            Delay = 0.75f,
            Scene = _spawnPoint.SceneName,
            DoorIndex = 0,
        },
        afterTransition: async () =>
        {
            await _fadeController.FadeOut(0.5f);
        }
        , beforeTransition: async () =>
        {
            var points = FindObjectsByType<SpawnPointController>(FindObjectsSortMode.None);
            foreach (var item in points)
            {
                if (item.Id == _spawnPoint.HashId)
                {
                    FindFirstObjectByType<PlayerController>().transform.SetPositionAndRotation(item.Target.position, item.Target.rotation);
                    await _fadeController.FadeIn(0.5f);
                    break;
                }
            }
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
