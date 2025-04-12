using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ChestController : MonoBehaviour,IInteract
{
    public UnityEvent OnOpen;
    private bool _isEmpty = false;
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _amount;
    [Inject] private GameplayController _gameplayController;
    [SerializeField] private Transform _lid;

    public async Task Execute()
    {
        await Open();
    }

    private async Task Open()
    {
        if (_isEmpty)
            return;
        OnOpen?.Invoke();
        await _gameplayController.AddItemToInventory(_item, _amount);
        _isEmpty = true;
    }

    public Transform GetTarget()
    {
        return transform;
    }
}
