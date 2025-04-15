using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    // M�quina de estado simples para o jogo
    private enum GameState
    {
        Playing,
        Paused,
        GameOver,
        GameComplete
    }
    private GameState currentState = GameState.Playing;

    [Header("Refer�ncia aos objetos")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject stageFinish;

    [Header("Refer�ncia �s telas")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCompletePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Configura��es")]
    public float interactionRadius = 2f;

    // Flag para evitar acionar game complete logo ap�s retomar
    private bool justResumed = false;

    void Start()
    {
        if (playerModel == null || gameOverPanel == null)
        {
            Debug.LogWarning("Player model ou GameOver panel n�o identificado no GameStateController. O fluxo de telas pode n�o funcionar...");
        }

        // Certifica-se de que as telas iniciais estejam desativadas
        gameOverPanel.SetActive(false);
        gameCompletePanel.SetActive(false);
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Atualiza os estados apenas se o jogo estiver ativo (Playing ou Paused)
        if (currentState == GameState.Playing || currentState == GameState.Paused)
        {
            // Se o jogador morreu, muda para GameOver
            int playerLife = playerModel.currentHealth;
            if (playerLife <= 0 && currentState != GameState.GameOver)
            {
                currentState = GameState.GameOver;
                gameOverPanel.SetActive(true);
                Time.timeScale = 1f;
            }

            CheckPauseInput();
            CheckGameFinish(stageFinish);
        }
    }

    #region Controles de Tela
    /// <summary>
    /// M�todo chamado pelo bot�o "Continuar" para retomar o jogo.
    /// </summary>
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            pausePanel.SetActive(false);
            controlsPanel.SetActive(false);
            Time.timeScale = 1f;
            // Protege a verifica��o do fim de jogo por 2 segundos
            justResumed = true;
            StartCoroutine(ResetJustResumed());
        }
    }

    private IEnumerator ResetJustResumed()
    {
        yield return new WaitForSeconds(2f);
        justResumed = false;
    }

    public void OpenControlsPanel()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void CloseControlsPanel()
    {
        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
    #endregion

    #region Pause
    private void CheckPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Se os controles estiverem abertos, fecha-os e retoma o jogo
            if (controlsPanel.activeSelf)
            {
                ResumeGame();
            }
            else if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        currentState = GameState.Paused;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    #endregion

    #region Fim de Jogo
    private void CheckGameFinish(GameObject interactible)
    {
        if (interactible == null) return;
        if (!interactible.activeInHierarchy) return;

        float distance = Vector3.Distance(player.transform.position, interactible.transform.position);
        // S� dispara o fim de jogo se estiver no estado Playing e se a flag justResumed for false
        if (distance <= interactionRadius && currentState == GameState.Playing && !justResumed)
        {
            currentState = GameState.GameComplete;
            gameCompletePanel.SetActive(true);
        }
    }
    #endregion
}
