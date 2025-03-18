using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor.VersionControl;

public class CollectiblesController : MonoBehaviour
{
    [Header("Referências dos Itens Coletáveis")]
    [Tooltip("Referência ao GameObject do jogador (tag 'Player').")]
    public GameObject player;

    [Tooltip("Referência ao GameObject da faca (tag 'Knife').")]
    public GameObject dagger;

    [Tooltip("Referência ao GameObject da poção (tag 'Potion').")]
    public GameObject potion;

    [Tooltip("Raio de alcance para coletar os itens.")]
    public float collectionRadius = 2f;

    [Header("UI")]
    [Tooltip("Texto que exibe mensagens de coleta.")]
    [SerializeField] private TextMeshProUGUI collectMessageText;

    [Tooltip("Texto que exibe mensagens de coleta.")]
    public TextMeshProUGUI itemCollectedMessageText;



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
        CheckCollectible(potion, "<Poção>");
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
                //Zerar a mensagem de coleta
                HideCollectionMessage();
               
                //Player coleta o item
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.StartCoroutine(playerController.CatchItemRoutine(collectible));
                    Debug.Log($"{collectibleName} sendo coletado!");
                    // Mensagem de coleta
                    ShowItemCollectedMessage($"Coletou {collectibleName}");
                    StartCoroutine(DelayToReadMessage(5f));


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

    // Para mostrar a mensagem de item coletado
   public void ShowItemCollectedMessage(string message)
    {
        if (!itemCollectedMessageText) return;
        itemCollectedMessageText.gameObject.SetActive(true);
        itemCollectedMessageText.text = message;
    }
    // Oculta e reseta a mensagem de item coletado
    public IEnumerator DelayToReadMessage(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (itemCollectedMessageText != null)
        {
            itemCollectedMessageText.gameObject.SetActive(false);
            itemCollectedMessageText.text = "";
            Debug.Log("Escondendo mensegem");
        }

    }
 


}
