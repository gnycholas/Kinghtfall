using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private AudioClip[] _audios;
    public override void InstallBindings()
    {
        Container.Bind<GameInputs>().FromMethod(SetupInput).AsSingle();
    }

    private GameInputs SetupInput()
    {
        var input = new GameInputs();
        input.Enable();
        return input;
    }
}
