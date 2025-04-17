using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameplayController : MonoBehaviour
{ 
    [Inject] private InventoryController _inventory;
    [Inject] private NotificationController _notificationController;
    [Inject] private PlayerController _playerController;
    [Inject] private IUIFactory _uiFactory;
    [Inject] private FadeController _fadeController;
    public InventoryController Inventory => _inventory;

    public PlayerController PlayerController { get => _playerController;}

    private void OnEnable()
    {
        _playerController.OnCollectItem.AddListener(_inventory.Collect);
        _playerController.OnConsumeStart.AddListener(_inventory.ConsumeItem);

        _playerController.OnDead.AddListener(GameOver);

        _inventory.OnWeaponEquip.AddListener(_playerController.OnEquipWeapon);
        _inventory.OnWeaponUnEquip.AddListener(_playerController.OnUnEquipWeapon);

        _inventory.OnConsumibleEquip.AddListener(_playerController.OnEquipItem);
        _inventory.OnWeaponUnEquip.AddListener(_playerController.OnUnEquipItem);  
    }
    private void Start()
    {
        _fadeController.FadeIn(0.5f);
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
        _notificationController?.ShowNotification(result.Item2);
        return result.Item1;
    }

    public async Task AddItemToInventory(ItemSO item, int amount)
    {
        _notificationController.ShowNotification($"{item.Name} obtido");
        _playerController.AddItemToInventory(item, amount);
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _notificationController.HiddenNotification();
    }
    public async void GameOver()
    {
        await GameOverAsync();
    }
    private async Task GameOverAsync()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        _uiFactory.Create("GameOver");
    } 
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
public struct NotificationParams
{
    
}
