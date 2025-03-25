using System.Collections;
using UnityEditor.VersionControl;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Configura��o do Ba�")]
    [Tooltip("Item que ser� armazenado no ba� (ex.: adaga).")]
    [SerializeField] private GameObject storedItem;


    [Tooltip("Animator que controla a anima��o do ba� (geralmente no GameObject 'Pivot').")]
    [SerializeField] private Animator chestAnimator;

    [Tooltip("Nome do trigger que dispara a anima��o de abertura no Animator.")]
    [SerializeField] private string openTriggerName = "OpenChest";

    [Tooltip("Tempo de espera ap�s disparar a anima��o para adicionar o item ao invent�rio.")]
    [SerializeField] private float addItemDelay = 1f;

    [Tooltip("Controle dos colet�veis para exibir mensagem de coleta")]
    [SerializeField] CollectiblesController collectiblesController;

    // Flag para evitar m�ltiplas intera��es
    private bool isOpened = false;

    [Header("Refer�ncia ao Jogador")]
    [Tooltip("Refer�ncia ao GameObject do jogador (com o PlayerController).")]
    [SerializeField] private GameObject player;
    private PlayerController playerController;


    private void Start()
    {
        // Se o jogador n�o foi atribu�do no Inspector, tenta encontr�-lo pela tag "Player"
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

    }
     

    // M�todo a ser chamado quando o jogador interage com o ba�
    public void Interact()
    {
        if (isOpened)
            return;

        // Dispara a anima��o de abertura no Animator
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger(openTriggerName);
        }
        isOpened = true;

        // Ativa o estado de catching no player para tocar a anima��o de coleta
        if (playerController != null)
        {
            playerController.SetCatching(true);
        }


        // Inicia a coroutine que, ap�s um delay, adiciona o item ao invent�rio
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
        // Opcional: desativa o objeto do item no ba� para que ele n�o seja coletado novamente
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
