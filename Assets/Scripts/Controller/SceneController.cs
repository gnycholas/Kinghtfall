using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneController : MonoBehaviour
{
    [Inject] private FadeController _fadeController;
    private async void Start()
    {
        await _fadeController.FadeIn(0.5f);
    } 
}
