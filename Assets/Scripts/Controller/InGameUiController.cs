using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUiController : MonoBehaviour
{
    public PlayerModel playerModel;
    public GhoulPatrolModel ghoulModel;

    public GameObject gameCompletePanel; // Painel de jogo completo
    public GameObject gameOverPanel; // Painel de fim de jogo

    // Start is called before the first frame update
    void Start()
    {
        if (gameCompletePanel.activeInHierarchy || gameOverPanel.activeInHierarchy
            )
        {
            gameCompletePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAgain()
    {
        if (gameCompletePanel.activeInHierarchy || gameOverPanel.activeInHierarchy)
        {
            gameCompletePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

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

        ghoulModel.currentHealth = ghoulModel.maxHealth;

        // Zera o inventário
        playerModel.inventory.Clear();

        SceneManager.LoadScene("GameDemo");
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
