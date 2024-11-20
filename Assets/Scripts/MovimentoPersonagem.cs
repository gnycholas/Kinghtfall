using UnityEngine;

public class MovimentoPersonagem : MonoBehaviour
{

    private CharacterController characterController;
    private Animator animator;

    private float velocidade;
    private Vector3 gravidade = new Vector3 (0f, -9.81f, 0f);

    private bool estaOrando;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        velocidade = 0f;
        estaOrando = false;
    }

    // Update is called once per frame
    void Update()
    {
        float movimentoHorizontal = Input.GetAxis("Horizontal");
        float movimentoVertical = Input.GetAxis("Vertical");

        Vector3 movimento = new Vector3 (movimentoHorizontal, 0, movimentoVertical);

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
            if (movimento != Vector3.zero) // Caso esteja pressionando algum botão de movimentação...
            {
                if (Input.GetKey(KeyCode.LeftShift)) // Se estiver pressionando a tecla shift...
                {
                    velocidade = 4f; // Aumenta a velocidade para a corrida...
                }
                else
                {
                    velocidade = 2f; // Do contrário, mantém a velocidade de caminhada...
                }

                characterController.Move(movimento.normalized * velocidade * Time.deltaTime); // Invoca o componente Character controller para mover o personagem, conforme a direção e velocidade atual...

                Quaternion rotacaoAlvo = Quaternion.LookRotation(movimento);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, Time.deltaTime * 10f); // Altera a direção que o personagem está olhando...
            }
            else
            {
                velocidade = 0f; // Caso não esteja pressionando nenhum botão de movimentação, então sua velocidade é zero...
            }

            characterController.Move(gravidade * Time.deltaTime); // Aplica a gravidade...

            animator.SetBool("Parado", velocidade <= 0f);
            animator.SetBool("Andando", velocidade > 0f && velocidade <= 2f);
            animator.SetBool("Correndo", velocidade > 2f);
        }
        Debug.Log(estaOrando);
    }
}
