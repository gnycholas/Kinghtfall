using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private FadeController _fadeController; 

    public override void InstallBindings()
    {
        Container.Bind<SceneTransitionController>().AsSingle();
        Container.Bind<FadeController>().FromComponentOn(_fadeController.gameObject).AsCached();
        Container.Bind<SaveManager>().FromComponentsOn(gameObject).AsCached();
    }
}
