using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameObject[] _drops;
    [SerializeField] private ItemSO[] _items;
    [SerializeField] private GameObject _hudItemViewFactory;

    public override void InstallBindings()
    {
        Container.Bind<ItemSO[]>().FromInstance(_items).AsCached();
        Container.BindFactory<HUDInventoryItemView,HUDInventoryItemView.Factory>().FromComponentInNewPrefab(_hudItemViewFactory);
        Container.Bind<GameObject[]>().WithId("Drops").FromInstance(_drops).AsCached();
        Container.BindFactory<string,GameObject, DropFactory>().FromFactory<CustomDropFactory>();
        Container.BindFactory<string,ItemSO,ItemFactory>().FromFactory<CustomItemFactory>();
    }
}
