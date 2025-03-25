using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUiController : MonoBehaviour
{
    [Header("Models")]
    public PlayerModel playerModel;       // Arraste o ScriptableObject do Player
    public PlayerController playerController;
    public GhoulPatrolModel ghoulModel;   // Se for necessário 

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

        // Reseta todos os campos do playerModel e do ghoulModel
        playerModel.currentHealth = playerModel.maxHealth;
        playerModel.isDead = false;
        playerModel.isKnifeEquipped = false;
        playerModel.isAttacking = false;
        playerModel.isInjured = false;
        playerModel.isWalking = false;
        playerModel.isRunning = false;
        playerModel.isTurningLeft = false;
        playerModel.isTurningRight = false;
        playerModel.isDrinking = false; 

        // Zera o inventário
        playerModel.inventory.Clear();

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
