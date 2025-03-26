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

    private void Update()
    {
        if (collectiblesController !=null && collectiblesController.itemCollectedMessageText.isActiveAndEnabled)
        {
            StartCoroutine(collectiblesController.DelayToReadMessage(5f));
        }

    }

    // M�todo a ser chamado quando o jogador interage com o ba�
    public void Interact()
    {
         

    }
     
}
