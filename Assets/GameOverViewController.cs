using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class GameOverViewController : MonoBehaviour
{
    [Inject(Id = "TryAgain")] private Button _tryAgainButton;
    [Inject(Id = "MainMenu")] private Button _mainMenuButton;
    [Inject] private GameplayController _gameplayController;

    private void Start()
    {
        _tryAgainButton.onClick.AddListener(TryAgain);
        _mainMenuButton.onClick.AddListener(_gameplayController.MainMenu);
    } 

    private void TryAgain()
    {
        SceneManager.LoadScene("Game");
    }
}
