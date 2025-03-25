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

    void Update()
    {
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        // Exemplo de pseudo-lógica:
        // Se o PlayerController tiver algo como "HasPotionInInventory()" ou "inventory.Contains("potion")"
        bool hasPotion = playerController.HasPotionInInventory();
        bool hasKey = playerController.HasKeyInInventory();
        bool hasKnife = playerController.HasKnifeInInventory();

        if (hasPotion)
        {
            uiView.ShowItem(
                "potion",                         // identificação do item
                uiModel.potionSprite,            // sprite do Model de UI
                playerModel.isPotionEquipped     // define se está equipado (alpha 100%)
            );
        }
        else
        {
            uiView.HideItem("potion");
        }

        if (hasKey)
        {
            uiView.ShowItem(
                "key",
                uiModel.keySprite,
                playerModel.isKeyEquipped
            );
        }
        else
        {
            uiView.HideItem("key");
        }

        if (hasKnife)
        {
            uiView.ShowItem(
                "knife",
                uiModel.daggerSprite,
                playerModel.isKnifeEquipped
            );
        }
        else
        {
            uiView.HideItem("knife");
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
