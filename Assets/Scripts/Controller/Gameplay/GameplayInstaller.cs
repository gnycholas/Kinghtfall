using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameObject _prefabLudvic;
    public override void InstallBindings()
    {
        Container.Bind<Inventory>().FromNew().AsCached();
        Container.Bind<GameObject>().WithId("Player").FromInstance(_prefabLudvic).AsSingle();
        Container.Bind<SpawnerController>().FromComponentInHierarchy().AsCached();
    }
}
