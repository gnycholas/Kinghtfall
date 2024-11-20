using UnityEngine;
using System.Collections.Generic;

public class MovimentoPersonagem : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    private float velocidade;
    private Vector3 gravidade = new Vector3(0f, -9.81f, 0f);

    private bool estaOrando;

    // Inventário do personagem
    public List<string> inventario = new List<string>(); // Lista simples armazenando nomes de itens

    // Start é chamado antes da primeira atualização
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        velocidade = 0f;
        estaOrando = false;
    }

    // Método para adicionar itens ao inventário
    public void AdicionarAoInventario(string nomeItem)
    {
        inventario.Add(nomeItem);
        Debug.Log("Item adicionado ao inventário: " + nomeItem);
    }

    // Update é chamado uma vez por frame
    void Update()
    {
        float movimentoHorizontal = Input.GetAxis("Horizontal");
        float movimentoVertical = Input.GetAxis("Vertical");

        Vector3 movimento = new Vector3(movimentoHorizontal, 0, movimentoVertical);

        if (Input.GetKeyDown(KeyCode.C))
        {
            estaOrando = true;
            animator.SetBool("Orando", true);
        {
            estaOrando = false;
            animator.SetBool("Orando", false);
        }

        if (!estaOrando)
        {
            if (movimento != Vector3.zero) // Se está movimentando...
            {
                if (Input.GetKey(KeyCode.LeftShift)) // Se está pressionando Shift...
                {
                    velocidade = 4f; // Velocidade de corrida
                }
                else
                {
                    velocidade = 2f; // Velocidade de caminhada
                }

                characterController.Move(movimento.normalized * velocidade * Time.deltaTime); // Move o personagem

                Quaternion rotacaoAlvo = Quaternion.LookRotation(movimento);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, Time.deltaTime * 10f); // Rotaciona o personagem
            }
            else
            {
                velocidade = 0f; // Sem movimento
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("Inventário:");
                foreach (string item in inventario)
                {
                    Debug.Log("- " + item);
                }
            }

            characterController.Move(gravidade * Time.deltaTime); // Aplica gravidade

            // Atualiza animações
            animator.SetBool("Parado", velocidade <= 0f);
            animator.SetBool("Andando", velocidade > 0f && velocidade <= 2f);
            animator.SetBool("Correndo", velocidade > 2f);
        }
       // Debug.Log("Está orando: " + estaOrando);
    }
}
