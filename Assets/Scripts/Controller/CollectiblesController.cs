using UnityEngine;

public class CollectiblesController : MonoBehaviour
{
    [Header("Referências dos Itens Coletáveis")]
    [Tooltip("Referência ao GameObject do jogador (tag 'Player').")]
    public GameObject player;

    [Tooltip("Referência ao GameObject da faca (tag 'Knife').")]
    public GameObject dagger;

    [Tooltip("Referência ao GameObject da poção (tag 'Potion').")]
    public GameObject potion;

    // Para a chave, vamos buscar dinamicamente os objetos com tag "Key".
    // public GameObject key; // Não usaremos essa referência estática.

    [Tooltip("Raio de alcance para coletar os itens.")]
    public float collectionRadius = 2f;

    private void Update()
    {
        if (player == null)
            return;

        CheckCollectible(dagger, "Dagger");
        CheckCollectible(potion, "Potion");
        CheckKeyCollectible();
    }

    private void CheckCollectible(GameObject collectible, string collectibleName)
    {
        if (collectible == null)
            return;

        // Verifica se o objeto está ativo na cena (se já foi coletado, ele estará desativado)
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

    private void CheckKeyCollectible()
    {
        // Busca todos os objetos ativos com a tag "Key" na cena
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in keys)
        {
            if (!key.activeInHierarchy)
                continue;

            float distance = Vector3.Distance(player.transform.position, key.transform.position);
            if (distance <= collectionRadius)
            {
                Debug.Log("Pressione E para coletar Key");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        bool added = playerController.AddItemToInventory(key);
                        if (added)
                        {
                            Debug.Log("Key coletada!");
                        }
                    }
                }
            }
        }
    }
}
