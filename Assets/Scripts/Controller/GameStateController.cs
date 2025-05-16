using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    // Máquina de estado simples para o jogo
    private enum GameState
    {
        Playing,
        Paused,
        GameOver,
        GameComplete
    }
    private GameState currentState = GameState.Playing;

    [Header("Referência aos objetos")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject stageFinish;

    [Header("Referência às telas")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCompletePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Configurações")]
    public float interactionRadius = 2f;

    // Flag para evitar acionar game complete logo após retomar
    private bool justResumed = false;

    void Start()
    {
        if (playerModel == null || gameOverPanel == null)
        {
            Debug.LogWarning("Player model ou GameOver panel não identificado no GameStateController. O fluxo de telas pode não funcionar...");
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
    /// Método chamado pelo botão "Continuar" para retomar o jogo.
    /// </summary>
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            pausePanel.SetActive(false);
            controlsPanel.SetActive(false);
            Time.timeScale = 1f;
            // Protege a verificação do fim de jogo por 2 segundos
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
        // Só dispara o fim de jogo se estiver no estado Playing e se a flag justResumed for false
        if (distance <= interactionRadius && currentState == GameState.Playing && !justResumed)
        {
            currentState = GameState.GameComplete;
            gameCompletePanel.SetActive(true);
        }
    }
    #endregion
}
