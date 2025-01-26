using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum GhoulState
{
    Patrol,     // Patrulhando aleatoriamente
    Idle,       // Parado esperando algum tempo
    Screaming,  // Em animação de grito antes de correr
    Chasing     // Correndo atrás do jogador
}

public class GhoulPatrolController : MonoBehaviour
{
    [Header("Referências MVC")]
    [SerializeField] private GhoulPatrolModel model;
    // Uma property para acessar de fora
    public GhoulPatrolModel Model => model;

    [SerializeField] private GhoulPatrolView view;
    [SerializeField] private NavMeshAgent agent;

    [Header("Referência ao Player")]
    [Tooltip("Arraste o transform do jogador aqui (ou encontre dinamicamente).")]
    [SerializeField] private Transform playerTransform;

    private GhoulState _currentState = GhoulState.Patrol;
    private float _chaseTimer;  // Contador para o tempo de perseguição

    // Para controle de patrulha
    private Vector3 _patrolCenterPosition;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (model.patrolCenter != null)
            _patrolCenterPosition = model.patrolCenter.position;
        else
            _patrolCenterPosition = transform.position;

        // Inicia no estado de patrulha
        EnterPatrolState();
    }

    private void Update()
    {
        // Atualiza de acordo com o estado atual
        switch (_currentState)
        {
            case GhoulState.Patrol:
                PatrolUpdate();
                DetectPlayer();
                break;

            case GhoulState.Idle:
                // no Idle, normalmente está rodando uma coroutine que controla o tempo
                DetectPlayer();
                break;

            case GhoulState.Screaming:
                // Fica parado ou aguarda a coroutine do scream
                // Pode verificar se player ainda está visível para continuar
                break;

            case GhoulState.Chasing:
                ChaseUpdate();
                break;
        }
    }

    #region Patrol & Idle
    private void EnterPatrolState()
    {
        _currentState = GhoulState.Patrol;
        agent.speed = model.walkSpeed;
        view.PlayWalkAnimation();
        ChooseNewDestination();
    }

    private void PatrolUpdate()
    {
        // Se chegou no destino
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Entra em Idle
            StartCoroutine(IdleRoutine());
        }
    }

    private IEnumerator IdleRoutine()
    {
        _currentState = GhoulState.Idle;
        view.PlayIdleAnimation();

        yield return new WaitForSeconds(model.idleTime);

        EnterPatrolState(); // volta a patrulhar
    }

    private void ChooseNewDestination()
    {
        float randomRadius = Random.Range(model.minRandomDistance, model.maxRandomDistance);
        Vector3 randomDirection = Random.insideUnitSphere * randomRadius + _patrolCenterPosition;
        randomDirection.y = _patrolCenterPosition.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, model.maxRandomDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Se não encontrou, tenta de novo
            ChooseNewDestination();
        }
    }
    #endregion

    #region Detection & Scream
    /// <summary>
    /// Verifica se o jogador está dentro do raio e sem obstáculos bloqueando a visão.
    /// </summary>
    private void DetectPlayer()
    {
        if (playerTransform == null) return;

        // 1) Verifica distância
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= model.detectionRadius)
        {
            // 2) (Opcional) Verifica ângulo de visão
            if (model.fieldOfViewAngle > 0)
            {
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);
                if (angle > model.fieldOfViewAngle * 0.5f)
                {
                    // jogador está fora do cone de visão
                    return;
                }
            }

            // 3) Verifica se há algum obstáculo entre o monstro e o jogador
            if (HasLineOfSightToPlayer())
            {
                // Se está em Patrol ou Idle, muda para Scream
                if (_currentState == GhoulState.Patrol || _currentState == GhoulState.Idle)
                {
                    StartCoroutine(ScreamRoutine());
                }
            }
        }
    }

    /// <summary>
    /// Faz um raycast (ou linecast) até o jogador para verificar obstrução.
    /// </summary>
    private bool HasLineOfSightToPlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 1.2f; // “olhos” do ghoul
        Vector3 target = playerTransform.position + Vector3.up * 1.2f;

        // Se o raycast bate em algo que não seja o player, considera sem visão
        if (Physics.Linecast(origin, target, out RaycastHit hit))
        {
            if (hit.transform == playerTransform)
                return true; // Tem visão
            else
                return false; // Algum objeto bloqueia
        }

        // Se não bateu em nada, provavelmente tem visão livre
        return true;
    }

    /// <summary>
    /// Inicia a animação de scream e aguarda o tempo configurado antes de iniciar a perseguição.
    /// </summary>
    private IEnumerator ScreamRoutine()
    {
        _currentState = GhoulState.Screaming;
        view.PlayScreamAnimation();
        agent.SetDestination(transform.position); // fica parado

        // Opcional: olha para o jogador
        LookAtPlayer();

        // Aguarda a duração do scream
        yield return new WaitForSeconds(model.screamDuration);

        // Passa para o chase
        EnterChaseState();
    }

    private void LookAtPlayer()
    {
        if (playerTransform == null) return;
        Vector3 lookPos = playerTransform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
    #endregion

    #region Chase
    private void EnterChaseState()
    {
        _currentState = GhoulState.Chasing;
        agent.speed = model.runSpeed;
        view.PlayRunAnimation();

        // Reseta timer
        _chaseTimer = 0f;
    }

    private void ChaseUpdate()
    {
        // Se não há player, sai
        if (playerTransform == null) return;

        // Continua atualizando o destino para o player
        agent.SetDestination(playerTransform.position);

        // Se perdemos a linha de visão, incrementamos um timer
        if (!HasLineOfSightToPlayer())
        {
            _chaseTimer += Time.deltaTime;
            // Se passou do tempo máximo, voltar para patrulha
            if (_chaseTimer >= model.chaseTimeout)
            {
                EnterPatrolState();
            }
        }
        else
        {
            // Se voltamos a ver o jogador, zera o timer
            _chaseTimer = 0f;
        }
    }
    #endregion
}
