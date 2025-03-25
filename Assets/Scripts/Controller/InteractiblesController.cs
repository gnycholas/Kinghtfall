using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Playables;

public class InteractiblesController : MonoBehaviour
{
    [Header("Referências dos Personagens")]
    public GameObject player;
    public GameObject ghoul;

    [Header("Referências dos Takes")]
    public PlayableDirector leverPlayableDirector;
    public PlayableDirector closedDoorPlayableDirector;
    public PlayableDirector openingDoorPlayableDirector;

    [Header("Referências dos Itens Interativos")]
    public GameObject lever;
    public GameObject leverDisabled;
    public GameObject exitDoor;
    public GameObject exitDoorOppened;
    public GameObject paper;
    public GameObject chest1;
    public GameObject chest2;

    [Tooltip("Raio de alcance para interagir com os objetos.")]
    public float interactionRadius = 2f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactiblesMessageText;


    // Flag para indicar se estamos escondendo a mensagem temporariamente
    private bool isHidingTemporarily = false;
    private GameInputs _inputs;
    private void Awake()
    {
        _inputs = new GameInputs(); 
    }
    private void OnDisable()
    {
        _inputs.Gameplay.Disable();
    }
    private void OnEnable()
    {
        _inputs.Gameplay.Enable();
    }

    private void Update()
    {
        if (player == null || ghoul == null) return;

      
        CheckCollectible(lever, "<Alavanca>");
        CheckCollectible(leverDisabled, "<Alavanca>");
        CheckCollectible(exitDoor, "<Porta de saída>");
        CheckCollectible(paper, "<Anotações>");
        CheckCollectible(chest1, "<Baú>");
        CheckCollectible(chest2, "<Baú>");
    }

    public void CheckCollectible(GameObject interactible, string interactibleName)
    {
        if (isHidingTemporarily) return;
        if (interactible == null) return;
        if (!interactible.activeInHierarchy) return;

        float distance = Vector3.SqrMagnitude(interactible.transform.position - player.transform.position);
        if (distance <= interactionRadius * interactionRadius)
        {
            // ALAVANCA
            if (interactible.CompareTag("Lever"))
            {
                ShowInteractiblesMessage($"Pressione E para interagir com {interactibleName}");
                if (_inputs.Gameplay.Interact.WasPressedThisFrame())
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
                        if (_inputs.Gameplay.Interact.WasPressedThisFrame())
                        {
                            HideInteractiblesMessage();
                            openingDoorPlayableDirector.Play();
                            interactible.tag = "Untagged";

                            // Desativa o MeshCollider para liberar a passagem
                            MeshCollider doorCollider = interactible.GetComponent<MeshCollider>();
                            if (doorCollider != null)
                            {
                                doorCollider.enabled = false;
                            }

                            Debug.Log("Abrindo porta!");
                            StartCoroutine(HideTextForSeconds(5f));
                        }
                    }
                    else
                    {
                        ShowInteractiblesMessage($"Pressione E para interagir com {interactibleName}");
                        if (_inputs.Gameplay.Interact.WasPressedThisFrame())
                        {
                            closedDoorPlayableDirector.Play();
                            StartCoroutine(HideTextForSeconds(3f));
                        }
                        Debug.Log("Ainda não coletou a chave...");
                    }
                }
            }
            // PAPEL
            else if (interactible.CompareTag("Paper"))
            {
                ShowInteractiblesMessage($"Pressione E para olhar {interactibleName}");
                if (_inputs.Gameplay.Interact.WasPressedThisFrame())
                {
                    Debug.Log("Lendo papel...");
                    // Aqui você pode abrir uma tela de texto ou outra interação
                    StartCoroutine(HideTextForSeconds(3f));
                }
            }
            // BAÚ
            else if (interactible.CompareTag("Chest"))
            {
                ShowInteractiblesMessage($"Pressione E para abrir {interactibleName}");
                if (_inputs.Gameplay.Interact.WasPressedThisFrame())
                {
                    // Busca o ChestController no GameObject do baú
                    ChestController chestController = interactible.GetComponent<ChestController>();
                    if (chestController != null)
                    {
                        chestController.Interact();

                    }
                    else
                    {
                        Debug.LogWarning("ChestController não encontrado no baú!");
                    }
                    // Altera o tag para evitar nova interação
                    interactible.tag = "Untagged";
                    HideInteractiblesMessage();
                    Debug.Log("Baú acionado!");
                    StartCoroutine(HideTextForSeconds(3f));
                }
            }
        }
    }

    private void ShowInteractiblesMessage(string message)
    {
        if (!interactiblesMessageText) return; 
        interactiblesMessageText.text = message;
    }

    private void HideInteractiblesMessage()
    {
        if (!interactiblesMessageText) return; 
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
