using System;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameObject[] _drops;
    [SerializeField] private ItemSO[] _items;
    [SerializeField] private GameObject _hudItemViewFactory;
    [SerializeField] private WeaponRef[] _weapons;
    [SerializeField] private ConsumibleRef[] _consumibles;
    [SerializeField] private UIRef[] _hudItems;

    public override void InstallBindings()
    {
        Container.Bind<UIRef[]>().FromInstance(_hudItems);
        Container.Bind<PauseController>().FromComponentInHierarchy().AsCached();
        Container.Bind<IUIFactory>().To<CustomUIFactory>().AsSingle();
        Container.Bind<ItemSO[]>().FromInstance(_items).AsSingle();
        Container.Bind<ConsumibleRef[]>().FromInstance(_consumibles).AsSingle();
        Container.Bind<WeaponRef[]>().FromInstance(_weapons).AsSingle();
        Container.BindFactory<HUDItemView, HUDItemView.Factory>().FromComponentInNewPrefab(_hudItemViewFactory); 
        Container.BindFactory<string, GameObject, DropFactory>().FromFactory<CustomDropFactory>(); 
        Container.Bind<GameObject[]>().FromInstance(_drops).AsSingle();
        Container.BindFactory<string,ItemSO,ItemFactory>().FromFactory<CustomItemFactory>();
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsCached();
        Container.Bind<InventoryController>().FromComponentInHierarchy().AsCached();
        Container.Bind<GameplayController>().FromMethod(FindOrCreate).AsCached();
        Container.Bind<NotificationController>().FromComponentInHierarchy().AsCached();
        Container.BindFactory<ItemSO, Weapon, WeaponFactory>().FromFactory<CustomWeaponFactory>();
        Container.BindFactory<ItemSO, Consumible, ConsumibleFactory>().FromFactory<CustomConsumibleFactory>();

    }

    private GameplayController FindOrCreate()
    {
        var gameplayController = FindAnyObjectByType<GameplayController>();
        if(gameplayController == null)
        {
            gameplayController = Container.InstantiateComponentOnNewGameObject<GameplayController>("GameplayController"); 
        }
        return gameplayController;
    }
}
