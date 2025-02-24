using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Playables;

public class InteractiblesController : MonoBehaviour
{
    [Header("Refer�ncias dos Personagens")]
    public GameObject player;
    public GameObject ghoul;

    [Header("Refer�ncias dos Takes")]
    public PlayableDirector leverPlayableDirector;
    public PlayableDirector closedDoorPlayableDirector;
    public PlayableDirector openingDoorPlayableDirector;

    [Header("Refer�ncias dos Itens Interativos")]
    public GameObject lever;
    public GameObject leverDisabled;
    public GameObject exitDoor;
    public GameObject exitDoorOppened;
    public GameObject paper;
    public GameObject chest; // Neste campo, atribua o GameObject "Pivot", que cont�m o Animator.

    [Tooltip("Raio de alcance para interagir com os objetos.")]
    public float interactionRadius = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactiblesMessageText;

    // Flag para indicar se estamos escondendo a mensagem temporariamente
    private bool isHidingTemporarily = false;

    private void Start()
    {
        if (interactiblesMessageText != null)
        {
            interactiblesMessageText.text = "";
            interactiblesMessageText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null || ghoul == null) return;

        // Se n�o estivermos no modo "escondendo temporariamente", limpa a mensagem
        if (!isHidingTemporarily)
        {
            HideInteractiblesMessage();
        }

        CheckCollectible(lever, "<Alavanca>");
        CheckCollectible(leverDisabled, "<Alavanca>");
        CheckCollectible(exitDoor, "<Porta de sa�da>");
        CheckCollectible(paper, "<Anota��es>");
        CheckCollectible(chest, "<Ba�>");
    }

    private void CheckCollectible(GameObject interactible, string interactibleName)
    {
        if (isHidingTemporarily) return;
        if (interactible == null) return;
        if (!interactible.activeInHierarchy) return;

        float distance = Vector3.Distance(player.transform.position, interactible.transform.position);
        if (distance <= interactionRadius)
        {
            // ALAVANCA
            if (interactible.CompareTag("Lever"))
            {
                ShowInteractiblesMessage($"Pressione E para interagir com {interactibleName}");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    leverPlayableDirector.Play();
                    interactible.SetActive(false);
                    leverDisabled.SetActive(true);
                    ghoul.SetActive(true);
                    StartCoroutine(HideTextForSeconds(5f));
                }
            }
            // PORTA
            else if (interactible.CompareTag("ExitDoor"))
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    bool collectedKey = playerController.HasKeyInInventory();
                    if (collectedKey)
                    {
                        ShowInteractiblesMessage($"Pressione E para interagir com {interactibleName}");
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            HideInteractiblesMessage();
                            openingDoorPlayableDirector.Play();
                            interactible.tag = "Untagged";
                            Debug.Log("Abrindo porta!");
                            StartCoroutine(HideTextForSeconds(5f));
                        }
                    }
                    else
                    {
                        ShowInteractiblesMessage($"Pressione E para interagir com {interactibleName}");
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            closedDoorPlayableDirector.Play();
                            StartCoroutine(HideTextForSeconds(3f));
                        }
                        Debug.Log("Ainda n�o coletou a chave...");
                    }
                }
            }
            // PAPEL
            else if (interactible.CompareTag("Paper"))
            {
                ShowInteractiblesMessage($"Pressione E para olhar {interactibleName}");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Lendo papel...");
                    // Aqui voc� pode abrir uma tela de texto ou outra intera��o
                    StartCoroutine(HideTextForSeconds(3f));
                }
            }
            // BA�
            else if (interactible.CompareTag("Chest"))
            {
                ShowInteractiblesMessage($"Pressione E para abrir {interactibleName}");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Obt�m o componente Animator no GameObject "Pivot" (que foi atribu�do no campo chest)
                    Animator chestAnimator = interactible.GetComponentInChildren<Animator>();
                    if (chestAnimator != null)
                    {
                        // Dispara o trigger que inicia a transi��o para a anima��o de abertura
                        chestAnimator.SetTrigger("OpenChest");
                    }
                    // Altera o tag para evitar nova intera��o
                    interactible.tag = "Untagged";
                    HideInteractiblesMessage();
                    Debug.Log("Ba� aberto!");
                    StartCoroutine(HideTextForSeconds(3f));
                }
            }
        }
    }

    private void ShowInteractiblesMessage(string message)
    {
        if (!interactiblesMessageText) return;
        interactiblesMessageText.gameObject.SetActive(true);
        interactiblesMessageText.text = message;
    }

    private void HideInteractiblesMessage()
    {
        if (!interactiblesMessageText) return;
        interactiblesMessageText.gameObject.SetActive(false);
        interactiblesMessageText.text = "";
    }

    private IEnumerator HideTextForSeconds(float seconds)
    {
        isHidingTemporarily = true;
        HideInteractiblesMessage();
        yield return new WaitForSeconds(seconds);
        isHidingTemporarily = false;
    }
}
