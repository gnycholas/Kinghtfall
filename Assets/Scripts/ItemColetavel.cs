using UnityEngine;
using UnityEngine.UI;

public class ItemColetavel : MonoBehaviour
{
    private bool jogadorPerto = false;
    public GameObject mensagemUI; // Referência à mensagem UI

    void Start()
    {
        // Garante que a mensagem está oculta no início
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
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            if (mensagemUI != null)
                mensagemUI.SetActive(false); // Oculta a mensagem
        }
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            // Ação ao coletar o item
            Debug.Log("Item coletado!");

            // Destrói o item
            Destroy(gameObject);

            // Oculta a mensagem
            if (mensagemUI != null)
                mensagemUI.SetActive(false);
        }
    }
}
