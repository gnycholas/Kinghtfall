using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameObject[] _drops;

    public override void InstallBindings()
    {
        Container.Bind<GameObject[]>().WithId("Drops").FromInstance(_drops).AsCached();
        Container.BindFactory<string,GameObject, DropFactory>().FromFactory<CustomDropFactory>();
    }
}
