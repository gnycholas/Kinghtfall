using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject panelExitConfirmation; // Painel de confirmação de saída
    public GameObject panelCredits; // Painel de créditos
    public GameObject mainMenuPanel; // Painel do menu principal
    public GameObject panelOptions; // Painel de opções
    public GameObject gameCompletePanel; // Painel de jogo completo

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelCredits != null && panelCredits.activeSelf)
            {
                CloseCredits();
            }
            else if (panelExitConfirmation != null && panelExitConfirmation.activeSelf)
            {
                CancelExit();
            }
            else if (panelOptions != null && panelOptions.activeSelf)
            {
                CloseOptions(); // Fecha o painel de opções ao pressionar ESC
            }
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadGame()
    {
        Debug.Log("Carregar Jogo - Implementação futura");
    }

    public void OpenOptions()
    {
        if (panelOptions != null && mainMenuPanel != null)
        {
            panelOptions.SetActive(true); // Ativa o painel de opções
            mainMenuPanel.SetActive(false); // Desativa o menu principal
        }
        else
        {
            Debug.LogError("Painel de opções ou menu principal não atribuído no Inspector.");
        }
    }

    public void CloseOptions()
    {
        if (panelOptions != null && mainMenuPanel != null)
        {
            panelOptions.SetActive(false); // Desativa o painel de opções
            mainMenuPanel.SetActive(true); // Ativa o menu principal
        }
        else
        {
            Debug.LogError("Painel de opções ou menu principal não atribuído no Inspector.");
        }
    }

    public void OpenCredits()
    {
        if (panelCredits != null && mainMenuPanel != null)
        {
            panelCredits.SetActive(true);
            mainMenuPanel.SetActive(false); // Esconde o menu principal
        }
        else
        {
            Debug.LogError("Painel de créditos ou menu principal não atribuído no Inspector.");
        }
    }

    public void CloseCredits()
    {
        if (panelCredits != null && mainMenuPanel != null)
        {
            panelCredits.SetActive(false);
            mainMenuPanel.SetActive(true); // Volta ao menu principal
        }
        else
        {
            Debug.LogError("Painel de créditos ou menu principal não atribuído no Inspector.");
        }
    }

    public void OpenExitConfirmation()
    {
        if (panelExitConfirmation != null && mainMenuPanel != null)
        {
            panelExitConfirmation.SetActive(true); // Mostra o painel de confirmação
            mainMenuPanel.SetActive(false); // Esconde o menu principal
        }
        else
        {
            Debug.LogError("Painel de confirmação de saída ou menu principal não atribuído no Inspector.");
        }
    }

    public void ConfirmExit()
    {
        Debug.Log("Saindo do jogo..."); // Mensagem de debug

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Para a execução no Editor
        #else
            Application.Quit(); // Fecha o jogo no build
        #endif
    }

    public void CancelExit()
    {
        if (panelExitConfirmation != null && mainMenuPanel != null)
        {
            panelExitConfirmation.SetActive(false); // Esconde o painel de confirmação
            mainMenuPanel.SetActive(true); // Volta ao menu principal
        }
        else
        {
            Debug.LogError("Painel de confirmação de saída ou menu principal não atribuído no Inspector.");
        }
    }
}