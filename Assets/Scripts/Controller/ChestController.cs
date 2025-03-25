using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Configuração do Baú")]
    [Tooltip("Item que será armazenado no baú (ex.: adaga).")]
    [SerializeField] private GameObject storedItem;


    [Tooltip("Animator que controla a animação do baú (geralmente no GameObject 'Pivot').")]
    [SerializeField] private Animator chestAnimator;

    [Tooltip("Nome do trigger que dispara a animação de abertura no Animator.")]
    [SerializeField] private string openTriggerName = "OpenChest";

    [Tooltip("Tempo de espera após disparar a animação para adicionar o item ao inventário.")]
    [SerializeField] private float addItemDelay = 1f;

    [Tooltip("Controle dos coletáveis para exibir mensagem de coleta")]
    [SerializeField] CollectiblesController collectiblesController;

    // Flag para evitar múltiplas interações
    private bool isOpened = false;

    [Header("Referência ao Jogador")]
    [Tooltip("Referência ao GameObject do jogador (com o PlayerController).")]
    [SerializeField] private GameObject player;
    private PlayerController playerController;


    private void Start()
    {
        // Se o jogador não foi atribuído no Inspector, tenta encontrá-lo pela tag "Player"
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

    }
     

    // Método a ser chamado quando o jogador interage com o baú
    public void Interact()
    {
        if (isOpened)
            return;

        // Dispara a animação de abertura no Animator
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger(openTriggerName);
        }
        isOpened = true;

        // Ativa o estado de catching no player para tocar a animação de coleta
        if (playerController != null)
        {
            playerController.SetCatching(true);
        }


        // Inicia a coroutine que, após um delay, adiciona o item ao inventário
        StartCoroutine(AddItemAfterDelay(addItemDelay));

    }

    private IEnumerator AddItemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerController != null && storedItem != null)
        {
            Debug.Log("Tentando adicionar o item: " + storedItem.name);
            playerController.AddItemToInventory(storedItem);
            // Mostrar mensagem de coleta
            collectiblesController.ShowItemCollectedMessage($"Coletou {storedItem.name}");
            
        }
        // Opcional: desativa o objeto do item no baú para que ele não seja coletado novamente
        if (storedItem != null)
        {
            storedItem.SetActive(false);
        }
        // Desativa o estado de catching no player
        if (playerController != null)
        {
            playerController.SetCatching(false);
        }
        yield return (collectiblesController.DelayToReadMessage(5f));

    }


}
