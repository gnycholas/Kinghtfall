using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameObject _prefabLudvic;
    [SerializeField] private WeaponRef[] _weaponsRef;
    public override void InstallBindings()
    {
        Container.Bind<Inventory>().FromNew().AsCached();
        Container.Bind<WeaponRef[]>().FromInstance(_weaponsRef).WithConcreteId("Weapon"); ;
        Container.BindFactory<Item,WeaponView, WeaponFactory>().FromFactory<CustomWeaponFactory>();
        Container.BindFactory<PlayerController,PlayerController.Factory>().WithId("Player").FromComponentInNewPrefab(_prefabLudvic).AsSingle();
        Container.Bind<SpawnerController>().FromComponentInHierarchy().AsCached();
    }
}

[System.Serializable]
public class WeaponRef
{
    public string Name;
    public AssetReference Ref;
}
