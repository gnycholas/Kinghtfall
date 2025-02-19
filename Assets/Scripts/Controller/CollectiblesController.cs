using UnityEngine;

public class CollectiblesController : MonoBehaviour
{
    [Header("Referências dos Itens Coletáveis")]
    [Tooltip("Referência ao GameObject do jogador (tag 'Player').")]
    public GameObject player;

    [Tooltip("Referência ao GameObject da poção (tag 'Potion').")]
    public GameObject item;

    [Tooltip("Raio de alcance para coletar a poção.")]
    public float collectionRadius = 2f;

    private void Update()
    {
        if (player == null || item == null)
            return;

        float distance = Vector3.Distance(player.transform.position, item.transform.position);
        if (distance <= collectionRadius)
        {
            Debug.Log("Aperte a tecla E para coletar a poção");

            if (Input.GetKeyDown(KeyCode.E))
            {
                // Supondo que o PlayerController tenha um método AddItemToInventory(GameObject item)
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.AddItemToInventory(item);
                }
            }
        }
    }
}
