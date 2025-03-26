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
        _inputs.Gameplay.Enable();
    }
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
        
    }

    public void CheckCollectible(GameObject interactible, string interactibleName)
    {
         
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
