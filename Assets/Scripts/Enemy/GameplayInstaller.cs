using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameObject[] _drops;
    [SerializeField] private ItemSO[] _items;
    [SerializeField] private GameObject _hudItemViewFactory;
    [SerializeField] private WeaponRef[] _weapons;
    [SerializeField] private ConsumibleRef[] _consumibles;

    public override void InstallBindings()
    {
        Container.Bind<ItemSO[]>().FromInstance(_items).AsCached();
        Container.Bind<ConsumibleRef[]>().FromInstance(_consumibles).AsCached();
        Container.Bind<WeaponRef[]>().FromInstance(_weapons).AsCached();
        Container.BindFactory<HUDItemView, HUDItemView.Factory>().FromComponentInNewPrefab(_hudItemViewFactory);
        Container.Bind<GameObject[]>().WithId("Drops").FromInstance(_drops).AsCached();
        Container.BindFactory<string,GameObject, DropFactory>().FromFactory<CustomDropFactory>();
        Container.BindFactory<string,ItemSO,ItemFactory>().FromFactory<CustomItemFactory>();
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsCached();
        Container.Bind<InventoryController>().FromComponentInHierarchy().AsCached();
        Container.Bind<GameplayController>().FromComponentInHierarchy().AsCached();
        Container.Bind<NotificationController>().FromComponentInHierarchy().AsCached();
        Container.BindFactory<ItemSO, Weapon, WeaponFactory>().FromFactory<CustomWeaponFactory>();
        Container.BindFactory<ItemSO, Consumible, ConsumibleFactory>().FromFactory<CustomConsumibleFactory>();

    }
}
