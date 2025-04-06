using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class GameplayController : MonoBehaviour
{
    public UnityEvent<string,float,NotificationParams> OnNotify;
    [Inject] private InventoryController _inventory;
    [Inject] private PlayerController _playerController;

    public InventoryController Inventory => _inventory;

    public PlayerController PlayerController { get => _playerController;}

    private void OnEnable()
    {
        _playerController.OnCollectItem.AddListener(_inventory.Collect);
        _playerController.OnConsumeStart.AddListener(_inventory.ConsumeItem);

        _inventory.OnWeaponEquip.AddListener(_playerController.OnEquipWeapon);
        _inventory.OnWeaponUnEquip.AddListener(_playerController.OnUnEquipWeapon);

        _inventory.OnConsumibleEquip.AddListener(_playerController.OnEquipItem);
        _inventory.OnWeaponUnEquip.AddListener(_playerController.OnUnEquipItem);  
    }
    private void OnDisable()
    {
        _playerController.OnCollectItem.RemoveListener(_inventory.Collect);
        _playerController.OnConsumeStart.RemoveListener(_inventory.ConsumeItem);

        _inventory.OnWeaponEquip.RemoveListener(_playerController.OnEquipWeapon);
        _inventory.OnWeaponUnEquip.RemoveListener(_playerController.OnUnEquipWeapon); 

        _inventory.OnConsumibleEquip.RemoveListener(_playerController.OnEquipItem);
        _inventory.OnWeaponUnEquip.RemoveListener(_playerController.OnUnEquipItem);
    }

    public bool CheckItem(ItemSO item, int amount)
    {
        var result = _inventory.CheckItem(item, amount);
        OnNotify?.Invoke(result.Item2,3,default);
        return result.Item1;
    }

    public void AddItemToInventory(ItemSO item, int amount)
    {
        OnNotify?.Invoke($"{item.Name} obtido",3, default);
        _playerController.AddItemToInventory(item, amount);
    }
}
public struct NotificationParams
{
    
}
