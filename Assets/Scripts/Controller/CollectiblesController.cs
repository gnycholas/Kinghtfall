using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectiblesController : MonoBehaviour
{
    [Header("Referências dos Itens Coletáveis")]
    [Tooltip("Referência ao GameObject do jogador (tag 'Player').")]
    public GameObject player;

    [Tooltip("Referência ao GameObject da faca (tag 'Knife').")]
    public GameObject dagger;

    [Tooltip("Referência ao GameObject da poção (tag 'Potion').")]
    public GameObject potion;

    // Para a chave, vamos buscar dinamicamente os objetos com tag "Key".
    // public GameObject key; // Não usaremos essa referência estática.

    [Tooltip("Raio de alcance para coletar os itens.")]
    public float collectionRadius = 2f;

    [Header("UI")]
    [Tooltip("Texto que exibe mensagens de coleta.")]
    [SerializeField] private TextMeshProUGUI collectMessageText;
    // ou private TextMeshProUGUI collectMessageText; se usar TMP

    private void Start()
    {
        // Deixa o texto invisível ou vazio inicialmente
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

        // Antes de verificar itens, esconde/limpa o texto para não ficar sempre ativo
        HideCollectionMessage();

        CheckCollectible(dagger, "<Adaga>");
        CheckCollectible(potion, "<Poção>");
        CheckKeyCollectible();
    }

    private void CheckCollectible(GameObject collectible, string collectibleName)
    {
        if (collectible == null) return;
        if (!collectible.activeInHierarchy) return;

        float distance = Vector3.Distance(player.transform.position, collectible.transform.position);
        if (distance <= collectionRadius)
        {
            ShowCollectionMessage($"Pressione E para coletar {collectibleName}");

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    bool added = playerController.AddItemToInventory(collectible);
                    if (added)
                    {
                        Debug.Log($"{collectibleName} coletado!");
                        // Você pode também exibir uma mensagem rápida no texto do Canvas, se quiser.
                        // collectible.SetActive(false); se quiser desativar o item coletado na cena
                    }
                }
            }
        }
    }

    private void CheckKeyCollectible()
    {
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in keys)
        {
            if (!key.activeInHierarchy) continue;

            float distance = Vector3.Distance(player.transform.position, key.transform.position);
            if (distance <= collectionRadius)
            {
                ShowCollectionMessage("Pressione E para coletar Key");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        bool added = playerController.AddItemToInventory(key);
                        if (added)
                        {
                            Debug.Log("Key coletada!");
                            // key.SetActive(false); se quiser desativar a key coletada
                        }
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
