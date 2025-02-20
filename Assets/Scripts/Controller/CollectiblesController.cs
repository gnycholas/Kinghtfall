using UnityEngine;

public class CollectiblesController : MonoBehaviour
{
    [Header("Refer�ncias dos Itens Colet�veis")]
    [Tooltip("Refer�ncia ao GameObject do jogador (tag 'Player').")]
    public GameObject player;

    [Tooltip("Refer�ncia ao GameObject da po��o (tag 'Potion').")]
    public GameObject item;

    [Tooltip("Raio de alcance para coletar a po��o.")]
    public float collectionRadius = 2f;

    private void Update()
    {
        if (player == null || item == null)
            return;

        float distance = Vector3.Distance(player.transform.position, item.transform.position);
        if (distance <= collectionRadius)
        {
            Debug.Log("Aperte a tecla E para coletar a po��o");

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Supondo que o PlayerController tenha um m�todo AddItemToInventory(GameObject item)
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.AddItemToInventory(item);
                }
            }
        }
    }
}
