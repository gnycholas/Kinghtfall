using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices; //TODO: Avaliar mudança para a Unity.Collection;

public class MovimentoPersonagem : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    private float velocidade;
    private Vector3 gravidade = new Vector3(0f, -9.81f, 0f);

    private bool estaOrando;

    [SerializeField] public Image staminaBar;
    [SerializeField] public float stamina, maxStamina, runCost, chargeRate;
    [SerializeField] private AudioSource passosAudioSource;
    [SerializeField] private AudioClip[] passosAudioClips; 

    private Coroutine recharge;

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

        characterController.Move(gravidade * Time.deltaTime); // Aplica gravidade

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Inventário:");
            foreach (string item in inventario)
            {
                Debug.Log("- " + item);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            estaOrando = true;
            animator.SetBool("Orando", true);
        } else if (Input.GetKeyUp(KeyCode.C))
        {
            estaOrando = false;
            animator.SetBool("Orando", false);
        }

        if (!estaOrando)
        {
            if (movimento != Vector3.zero) // Se está movimentando...
            {
                if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f) // Se está pressionando Shift...
                {
                    velocidade = 4f; // Velocidade de corrida

                    stamina -= runCost * Time.deltaTime; // Consome a estamina ao correr...
                    if (stamina < 0) stamina = 0;
                    staminaBar.fillAmount = stamina / maxStamina;

                    if (recharge != null) StopCoroutine(recharge); // Garante que a barra aguarde um segundo para que recarregue e que apenas uma corrotina seja executada por vez...
                    recharge = StartCoroutine(rechargeStamina());
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

            // Atualiza animações
            animator.SetBool("Parado", velocidade <= 0f);
            animator.SetBool("Andando", velocidade > 0f && velocidade <= 2f);
            animator.SetBool("Correndo", velocidade > 2f);
        }
    }

    private IEnumerator rechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (stamina < maxStamina)
        {
            stamina += chargeRate / 10f;
            if (stamina > maxStamina) stamina = maxStamina;
            staminaBar.fillAmount = stamina / maxStamina;
            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnPassos()
    {
       int index = Random.Range(0, passosAudioClips.Length);
       passosAudioSource.PlayOneShot(passosAudioClips[index]);
    }
}

