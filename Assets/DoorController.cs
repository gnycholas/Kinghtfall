using System.Threading.Tasks; 
using UnityEngine;
using UnityEngine.Events; 
using Zenject;

public class DoorController : MonoBehaviour,IInteract,IEnvironmentState
{
    public UnityEvent OnOpenDoor;
    private Transform _door;
    private bool _isOpen;
    [SerializeField] private int _doorIndex;
    [Inject] private GameplayController _gameplayController;
    [Inject] private SceneTransitionController _sceneTransitionController;
    [Inject] private FadeController _fadeController;
    [SerializeField] private Requirement _requirement;
    [SerializeField] private SpawnPointModel _spawnPoint;
    [SerializeField] private string _doorName;

    public int Hash => _doorName.GetHashCode();

    private void Awake()
    {
        _door = transform.GetChild(0);
    }
    private void OnTriggerEnter(Collider other)
    {
        _sceneTransitionController.LoadTransitionScene(new SceneLoadParams()
        {
            Delay = 0.75f,
            Scene = _spawnPoint.SceneName,
            DoorIndex = _doorIndex,
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
    public async Task Execute()
    {
        bool state = false;
        if (_requirement != null)
        {
            state = _requirement.Check(_gameplayController);
        }
        if (_isOpen || (_requirement == null || state))
        {
            if (string.IsNullOrEmpty(_doorName)) 
            { 
                GetComponent<EnviromentStateCheck>()?.ChangeState(Hash, true);
            } 
            await Open();
        }
    }

    private async Task Open()
    {
        OnOpenDoor?.Invoke();
        _gameplayController.PlayerController.ToggleMove(false);
        _sceneTransitionController.LoadTransitionScene(new SceneLoadParams()
        {
            Delay = 0.75f,
            Scene = _spawnPoint.SceneName,
            DoorIndex = _doorIndex,
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

    public void ChangeState(bool active)
    {
        _isOpen = active;
    }
}
