using System.Threading.Tasks;
using Cysharp.Threading.Tasks; 
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ChestController : MonoBehaviour,IInteract,IEnvironmentState
{
    public UnityEvent OnOpen;
    private bool _isEmpty = false;
    [SerializeField] private string _chestId;
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _amount;
    [Inject] private GameplayController _gameplayController;
    [SerializeField] private Transform _lid;
    [SerializeField] private AnimatorOverrideController _interact;

    public int Hash { get; private set; }

    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(_chestId))
        {
            Hash = _chestId.GetHashCode();
        }
    }

    public async Task Execute()
    {
        _gameplayController.PlayerController.ToggleMove(false);
        await Open();
        _gameplayController.PlayerController.ToggleMove(true); 
    }

    private async Task Open()
    {
        if (_isEmpty)
            return;
        GetComponent<EnviromentStateCheck>().ChangeState(Hash, true);
        OnOpen.Invoke();
        await Transition();
        await _gameplayController.AddItemToInventory(_item, _amount);
        _isEmpty = true;
    }

    public Transform GetTarget()
    {
        return transform;
    }
    private async Task Transition()
    {
        var start = _lid.localEulerAngles;
        var end = new Vector3(-70, start.y, start.z);
        float elapsedTime = 0;
        while (true)
        {
            elapsedTime += Time.deltaTime/2;
            _lid.localRotation = Quaternion.Lerp(Quaternion.Euler(start), Quaternion.Euler(end), elapsedTime);
            if(elapsedTime >= 1)
            {
                break;
            }
            await UniTask.NextFrame();
        }
    }

    public AnimatorOverrideController GetInteraction()
    {
        return _interact;
    }

    public void ChangeState(bool active)
    {
        if (active)
        {
            _isEmpty = true;
            Transition();
        } 
    }
}
