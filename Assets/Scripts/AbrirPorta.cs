using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbrirPorta : MonoBehaviour
{
    [SerializeField] private Animator animatorPorta;
    [SerializeField] private string itemNecessario;

    private bool jogadorPerto = false;
    private MovimentoPersonagem playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;
            playerMovement = other.GetComponent<MovimentoPersonagem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            playerMovement = null;
        }
    }

    private void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            if (playerMovement.inventario.Contains(itemNecessario))
            {
                animatorPorta.Play("animationDoor", 0, 0.0f);
                Debug.Log("Porta aberta!");
            }
            else
            {
                Debug.Log("Você não tem a chave para abrir esta porta!");
            }
        }
    }
}


