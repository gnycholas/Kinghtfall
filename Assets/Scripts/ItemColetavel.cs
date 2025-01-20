using System.Collections.Generic;
using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    private bool jogadorPerto = false;
    public GameObject mensagemUI; // Referência à mensagem UI

    private MovimentoPersonagem playerMovement; // Referencia ao script do personagem

    void Start()
    {
        // Garante que a mensagem esta oculta no início
        if (mensagemUI != null)
            mensagemUI.SetActive(false);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            jogadorPerto = true;
            if (mensagemUI != null)
                mensagemUI.SetActive(true); // Mostra a mensagem

            // Obtém a referencia ao script do personagem
            playerMovement = other.GetComponent<MovimentoPersonagem>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            if (mensagemUI != null)
                mensagemUI.SetActive(false); // Oculta a mensagem

            playerMovement = null; // Remove a referência ao personagem
        }
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            if (playerMovement != null)
            {
                // Adiciona o item ao inventário do personagem
                string nomeItem = gameObject.name; // Usa o nome do GameObject como identificador
                playerMovement.AdicionarAoInventario(nomeItem);
            }

            // Ação ao coletar o item
            Debug.Log("Item coletado: " + gameObject.name);

            // Destrói o item
            Destroy(gameObject);

            // Oculta a mensagem
            if (mensagemUI != null)
                mensagemUI.SetActive(false);
        }
    }
}
