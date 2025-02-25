using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    [Header("Referência aos objetos")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject stageFinish;
    
    [Header("Referência às telas")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCompletePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;

    private bool isPaused = false;
    public float interactionRadius = 2f;

    // Start is called before the first frame update
    void Start()
    {
        if (playerModel == null || gameOverPanel == null)
        {
            Debug.Log("Player model ou Game over panel não identificado no Game state controller. O fluxo de telas pode não funcionar...");
        }
    }

    // Update is called once per frame
    void Update()
    {
        int playerLife = playerModel.currentHealth;

        if (playerLife <= 0)
        {
            gameOverPanel.SetActive(true);
        }

        CheckPauseInput();
        CheckGameFinish(stageFinish);
    }

    public void OpenControlsPanel()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void CloseControlsPanel()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    #region Pause
    private void CheckPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        // Se estiver pausado, para o tempo do jogo.
        // Se não, volta ao normal.
        Time.timeScale = isPaused ? 0f : 1f;
    }
    #endregion

    #region Fim de jogo
    private void CheckGameFinish(GameObject interactible)
    {
        if (interactible == null) return;
        if (!interactible.activeInHierarchy) return;

        float distance = Vector3.Distance(player.transform.position, interactible.transform.position);

        if (distance <= interactionRadius)
        {
            gameCompletePanel.SetActive(true);
        }
    }
    #endregion
}
