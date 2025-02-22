using UnityEngine;
using TMPro;

public class CollectiblesController : MonoBehaviour
{
    [Header("Refer�ncias dos Itens Colet�veis")]
    [Tooltip("Refer�ncia ao GameObject do jogador (tag 'Player').")]
    public GameObject player;

    [Tooltip("Refer�ncia ao GameObject da faca (tag 'Knife').")]
    public GameObject dagger;

    [Tooltip("Refer�ncia ao GameObject da po��o (tag 'Potion').")]
    public GameObject potion;

    [Tooltip("Raio de alcance para coletar os itens.")]
    public float collectionRadius = 2f;

    [Header("UI")]
    [Tooltip("Texto que exibe mensagens de coleta.")]
    [SerializeField] private TextMeshProUGUI collectMessageText;

    private void Start()
    {
        if (collectMessageText != null)
        {
            collectMessageText.text = "";
            collectMessageText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        HideCollectionMessage();

        CheckCollectible(dagger, "<Adaga>");
        CheckCollectible(potion, "<Po��o>");
        CheckKeyCollectible();
    }

    private void CheckCollectible(GameObject collectible, string collectibleName)
    {
        if (collectible == null)
            return;
        if (!collectible.activeInHierarchy)
            return;

        float distance = Vector3.Distance(player.transform.position, collectible.transform.position);
        if (distance <= collectionRadius)
        {
            ShowCollectionMessage($"Pressione E para coletar {collectibleName}");
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.StartCoroutine(playerController.CatchItemRoutine(collectible));
                    Debug.Log($"{collectibleName} sendo coletado!");
                }
            }
        }
    }

    private void CheckKeyCollectible()
    {
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in keys)
        {
            if (!key.activeInHierarchy)
                continue;
            float distance = Vector3.Distance(player.transform.position, key.transform.position);
            if (distance <= collectionRadius)
            {
                ShowCollectionMessage("Pressione E para coletar Key");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.StartCoroutine(playerController.CatchItemRoutine(key));
                        Debug.Log("Key sendo coletada!");
                    }
                }
            }
        }
    }

    private void ShowCollectionMessage(string message)
    {
        if (collectMessageText == null) return;
        collectMessageText.gameObject.SetActive(true);
        collectMessageText.text = message;
    }

    private void HideCollectionMessage()
    {
        if (collectMessageText == null) return;
        collectMessageText.gameObject.SetActive(false);
        collectMessageText.text = "";
    }
}
