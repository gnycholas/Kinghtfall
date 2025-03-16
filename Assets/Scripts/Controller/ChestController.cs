using System.Collections;
using UnityEngine;
using Zenject;

public class ChestController : MonoBehaviour
{
    [Header("Configuração do Baú")]
    [Tooltip("Item que será armazenado no baú (ex.: adaga).")]
    [SerializeField] private ItemView storedItem;

    [Tooltip("Animator que controla a animação do baú (geralmente no GameObject 'Pivot').")]
    [SerializeField] private Animator chestAnimator;

    [Tooltip("Nome do trigger que dispara a animação de abertura no Animator.")]
    [SerializeField] private string openTriggerName = "OpenChest";

    [Tooltip("Tempo de espera após disparar a animação para adicionar o item ao inventário.")]
    [SerializeField] private float addItemDelay = 1f;

    // Flag para evitar múltiplas interações
    private bool isOpened = false; 
    [Inject] private SpawnerController _spawner;
    [SerializeField] private AnimatorOverrideController _animation;
    [SerializeField] private float _time; 

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
 
        StartCoroutine(AddItemAfterDelay(addItemDelay));
    }

    private IEnumerator AddItemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _spawner.Player.AddItemToInventory(storedItem,_animation,_time); 
        if (storedItem != null)
        {
            storedItem.gameObject.SetActive(false);
        } 
    }
}
