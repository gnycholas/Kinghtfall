using System;
using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;
using Zenject;

public class ChestController : MonoBehaviour,IInteract
{
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _amount;
    [Inject] private GameplayController _gameplayController;

    public async System.Threading.Tasks.Task Execute()
    {
        await Open();
    }

    private async System.Threading.Tasks.Task Open()
    {
        _gameplayController.AddItemToInventory(_item, _amount);
    }

    public Transform GetTarget()
    {
        return transform;
    }
}
