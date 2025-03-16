using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameplayManager : MonoBehaviour
{
    [Inject] private Inventory _inventory;
    [Inject] private SpawnerController _spawnerController;

    private void Awake()
    {
        _spawnerController.OnSpawn += CollectItem;
        _spawnerController.OnDespawn += RemoveCollectItem;
    }

    private void RemoveCollectItem(PlayerController controller)
    {
        controller.OnCollectItem.RemoveListener(Collect);
    }

    private void CollectItem(PlayerController controller)
    {
        controller.OnCollectItem.AddListener(Collect);
    }

    private void Collect(Item arg0)
    {
        _inventory.AddItem(arg0);
    }
}
