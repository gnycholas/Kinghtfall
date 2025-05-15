using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseViewController : MonoBehaviour
{
    [Inject(Id = "Continue")] private Button _continueButton;
    [Inject(Id = "Options")] private Button _optionsButton;
    [Inject(Id = "Controls")] private Button _controlsButton;
    [Inject(Id = "MainMenu")] private Button _mainMenuButton;
    [Inject] private PauseController _pauseController;
    [Inject] private GameplayController _gameplayController;
    [Inject] private IUIFactory _uiFactory;
    private void Start()
    {
        _continueButton.onClick.AddListener(_pauseController.Resume);
        _controlsButton.onClick.AddListener(() => _uiFactory.Create("Controllers"));
        _optionsButton.onClick.AddListener(() => _uiFactory.Create("Options"));
        _mainMenuButton.onClick.AddListener(_gameplayController.MainMenu);
    }

}
