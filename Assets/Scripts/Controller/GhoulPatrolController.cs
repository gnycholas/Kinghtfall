using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GhoulPatrolController : MonoBehaviour
{
    [Header("Referências MVC")]
    [SerializeField] private GhoulPatrolModel model; // ScriptableObject com configs
    [SerializeField] private GhoulPatrolView view;   // Controla as animações
    [SerializeField] private NavMeshAgent agent;     // Referência ao NavMeshAgent

    private bool _isIdle;                            // Flag para indicar se está parado

    // Se quiser limitar a patrulha a uma área, você pode armazenar aqui a pos. inicial
    private Vector3 _patrolCenterPosition;

    private void Start()
    {
        // Garante que temos um agent associado
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        // Ajusta a velocidade do agent com base no Model
        agent.speed = model.walkSpeed;

        // Define o centro da patrulha
        if (model.patrolCenter != null)
        {
            _patrolCenterPosition = model.patrolCenter.position;
        }
        else
        {
            // Se não tiver um "patrolCenter" definido, usamos a posição inicial do Ghoul
            _patrolCenterPosition = transform.position;
        }

        // Escolhe o primeiro destino de patrulha
        ChooseNewDestination();
    }

    private void Update()
    {
        if (_isIdle) return;

        // Se o agente não estiver processando o caminho e tiver chegado ao destino
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Inicia a lógica de ficar parado
            StartCoroutine(IdleRoutine());
        }
    }

    /// <summary>
    /// Escolhe um ponto aleatório no NavMesh dentro de um raio e manda o agente para lá.
    /// </summary>
    private void ChooseNewDestination()
    {
        // Gera um ponto aleatório na esfera (ou círculo no plano) entre min e max
        var randomRadius = Random.Range(model.minRandomDistance, model.maxRandomDistance);
        var randomDirection = Random.insideUnitSphere * randomRadius;

        // Ajusta a altura e desloca em relação ao centro
        randomDirection += _patrolCenterPosition;
        randomDirection.y = _patrolCenterPosition.y; // mantém o y consistente (se desejar)

        NavMeshHit hit;
        // Tenta encontrar uma posição válida no NavMesh
        if (NavMesh.SamplePosition(randomDirection, out hit, model.maxRandomDistance, NavMesh.AllAreas))
        {
            // Se encontrou uma posição válida, manda o agente para lá
            _isIdle = false;
            agent.SetDestination(hit.position);

            // Toca a animação de caminhada
            view.PlayWalkAnimation();
        }
        else
        {
            // Se não encontrou, tenta novamente (cuidado para não cair em loop infinito)
            ChooseNewDestination();
        }
    }

    /// <summary>
    /// Rotina de Idle (ficar parado) por alguns segundos antes de definir nova rota.
    /// </summary>
    private IEnumerator IdleRoutine()
    {
        _isIdle = true;
        // Toca animação de Idle
        view.PlayIdleAnimation();

        // Espera pelo tempo configurado no Model
        yield return new WaitForSeconds(model.idleTime);

        // Após o tempo, escolhe outro ponto
        ChooseNewDestination();
    }
}
