using System.Collections;
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

        //Bug aqui 

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
