using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerAudioController : MonoBehaviour
{
    [Inject(Id = "Steps")] private AudioSource _stepSource;
    [Inject(Id = "Vfx")] private AudioSource _vfxSource;
    [Inject(Id ="Step")] private AudioClip _step;
    [Inject(Id = "Damage")] private AudioClip _takeDamage;
    [Inject(Id = "Die")] private AudioClip _die;
    [Inject(Id = "Run")] private AudioClip _run;
    [Inject] private PlayerController _playerController;

    public void OnEnable()
    {
        _playerController.OnPlayAudio.AddListener(PlayAudio);
    }
    private void OnDisable()
    {
        _playerController.OnPlayAudio.AddListener(PlayAudio);
    }

    public void PlayAudio(string name)
    {
        switch (name)
        {
            case "Step": _stepSource.PlayOneShot(_step); break;
            case "Die": _vfxSource.PlayOneShot(_die); break;
            case "Run": _stepSource.PlayOneShot(_run); break;
            case "Damage": _vfxSource.PlayOneShot(_takeDamage); break;
        }
    }
}
