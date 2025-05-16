using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private AudioClip[] _steps;
    [SerializeField] private AudioClip[] _stepsRun;
    [SerializeField] private AudioClip[] _takeDamage;
    [SerializeField] private AudioClip[] _attack;
    [SerializeField] private AudioClip _dying;


    public override void InstallBindings()
    {
        Container.Bind<GameInputs>().FromMethod(SetupInput).AsCached();
        Container.Bind<AudioClip[]>().WithId("AttackAudio").FromInstance(_attack).AsCached();
        Container.Bind<AudioClip[]>().WithId("StepAudio").FromInstance(_steps).AsCached();
        Container.Bind<AudioClip[]>().WithId("RunAudio").FromInstance(_stepsRun).AsCached();
        Container.Bind<AudioClip>().WithId("DieAudio").FromInstance(_dying).AsCached();
        Container.Bind<AudioClip[]>().WithId("DamageAudio").FromInstance(_takeDamage).AsCached();

    }

    private GameInputs SetupInput()
    {
        var input = new GameInputs();
        input.Enable();
        return input;
    }
}
