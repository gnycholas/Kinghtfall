using UnityEngine;

public class CollectiblesController : MonoBehaviour
{
    [Header("Refer�ncias dos Itens Colet�veis")]
    [Tooltip("Refer�ncia ao GameObject do jogador (tag 'Player').")]
    public GameObject player;

    [Tooltip("Refer�ncia ao GameObject da faca (tag 'Knife').")]
    public GameObject dagger;

    [Tooltip("Refer�ncia ao GameObject da po��o (tag 'Potion').")]
    public GameObject potion;

    [Tooltip("Refer�ncia ao GameObject da chave (tag 'Key').")]
    public GameObject key;

    [Tooltip("Raio de alcance para coletar os itens.")]
    public float collectionRadius = 2f;

    private void Update()
    {
        if (player == null)
            return;

        CheckCollectible(dagger, "Dagger");
        CheckCollectible(potion, "Potion");
        CheckCollectible(key, "Key");
    }

    private void CheckCollectible(GameObject collectible, string collectibleName)
    {
        if (collectible == null)
            return;

        // Apenas verifica se o objeto estiver ativo (se j� foi coletado, ele deve estar desativado)
        if (!collectible.activeInHierarchy)
            return;

        float distance = Vector3.Distance(player.transform.position, collectible.transform.position);
        if (distance <= collectionRadius)
        {
            Debug.Log($"Pressione E para coletar {collectibleName}");

            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    bool added = playerController.AddItemToInventory(collectible);
                    if (added)
                    {
                        Debug.Log($"{collectibleName} coletado!");
                    }
                }
            }
        }
    }
}
