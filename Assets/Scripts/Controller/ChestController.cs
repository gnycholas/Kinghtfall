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

    private void Update()
    {
        if (collectiblesController !=null && collectiblesController.itemCollectedMessageText.isActiveAndEnabled)
        {
            StartCoroutine(collectiblesController.DelayToReadMessage(5f));
        }

    }

    // Método a ser chamado quando o jogador interage com o baú
    public void Interact()
    {
         

    }
     
}
