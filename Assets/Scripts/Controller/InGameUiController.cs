using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUiController : MonoBehaviour
{
    [Header("Models")]
    public PlayerModel playerModel;       // Arraste o ScriptableObject do Player
    public PlayerController playerController;
    public GhoulPatrolModel ghoulModel;   // Se for necessário
    public InGameUiModel uiModel;         // Arraste o ScriptableObject de UI

    [Header("View")]
    public InGameUiView uiView;           // Arraste o objeto que tem o InGameUiView

    [Header("Paineis de tela")]
    public GameObject gameCompletePanel;
    public GameObject gameOverPanel;

    void Start()
    {
        if (gameCompletePanel.activeInHierarchy || gameOverPanel.activeInHierarchy)
        {
            gameCompletePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
    }
 
    public void PlayAgain()
    {
        if (gameCompletePanel.activeInHierarchy || gameOverPanel.activeInHierarchy)
        {
            gameCompletePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        } 
        SceneManager.LoadScene("Game");
    }

    public void MainMenu()
    {
        if (gameCompletePanel.activeInHierarchy || gameOverPanel.activeInHierarchy)
        {
            gameCompletePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        SceneManager.LoadScene("MainMenu");
    }
}
