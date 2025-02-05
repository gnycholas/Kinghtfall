using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum GhoulState
{
    Patrol,
    Idle,
    Screaming,
    Chasing,
    Attacking
}

public class GhoulPatrolController : MonoBehaviour
{
    [Header("MVC References")]
    [SerializeField] private GhoulPatrolModel model;
    public GhoulPatrolModel Model => model;

    [SerializeField] private GhoulPatrolView view;
    [SerializeField] private NavMeshAgent agent;

    [Header("Player")]
    [Tooltip("Arraste o Transform do jogador aqui ou busque dinamicamente.")]
    [SerializeField] private Transform playerTransform;

    private GhoulState _currentState = GhoulState.Patrol;

    // Para controle de patrulha
    private Vector3 _patrolCenter;
    private Coroutine _idleCoroutine;

    // Controle de chase
    private float _chaseTimer;

    // Controle do ataque contínuo
    private Coroutine damageCoroutine;  // Corrotina que aplica dano enquanto o player estiver no attackRange

    private void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        // Define o centro de patrulha
        if (model && model.patrolCenter)
        {
            _patrolCenter = model.patrolCenter.position;
        }
        else
        {
            // Se não houver um "patrolCenter" definido, usa posição inicial
            _patrolCenter = transform.position;
        }

        // Inicia no estado de Patrol
        EnterPatrolState();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case GhoulState.Patrol:
                PatrolUpdate();
                DetectPlayer();
                break;

            case GhoulState.Idle:
                // Idle é controlado por Coroutine, mas ainda podemos checar se o player apareceu
                DetectPlayer();
                break;

            case GhoulState.Screaming:
                // Nesse estado, o monstro está parado gritando (ScreamRoutine)
                // A detecção já ocorreu antes; você pode checar novamente se quiser
                break;

            case GhoulState.Chasing:
                ChaseUpdate();
                AttackCheck();    // Verifica se pode atacar
                break;

            case GhoulState.Attacking:
                // Dependendo do design, ficamos parados na animação de ataque
                // ou rodamos alguma lógica extra (ver AttackUpdate se necessário)
                break;
        }
    }

    #region Patrol & Idle
    private void EnterPatrolState()
    {
        _currentState = GhoulState.Patrol;

        if (agent) agent.speed = model.walkSpeed;
        if (view) view.PlayWalkAnimation();

        ChooseNewDestination();
    }

    private void PatrolUpdate()
    {
        // Checa se chegamos ao destino
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Entrar em Idle
            if (_idleCoroutine == null)
            {
                _idleCoroutine = StartCoroutine(IdleRoutine());
            }
        }
    }

    private IEnumerator IdleRoutine()
    {
        _currentState = GhoulState.Idle;

        if (view) view.PlayIdleAnimation();

        yield return new WaitForSeconds(model.idleTime);

        _idleCoroutine = null;
        EnterPatrolState(); // Volta a patrulhar
    }

    private void ChooseNewDestination()
    {
        float randomRadius = Random.Range(model.minRandomDistance, model.maxRandomDistance);
        Vector3 randomDir = Random.insideUnitSphere * randomRadius;
        randomDir += _patrolCenter;
        randomDir.y = _patrolCenter.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, model.maxRandomDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Se falhar, tenta novamente (cuidado com possível loop infinito)
            ChooseNewDestination();
        }
    }
    #endregion

    #region Detection
    private void DetectPlayer()
    {
        if (!playerTransform || !model) return;

        // 1) Distância
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > model.detectionRadius) return;

        // 2) Ângulo de visão (opcional)
        if (model.fieldOfViewAngle > 0)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle > model.fieldOfViewAngle * 0.5f)
                return; // Fora do cone
        }

        // 3) Linha de visão (Raycast/Linecast)
        if (HasLineOfSightToPlayer())
        {
            // Se está em Patrol ou Idle, inicia o Scream
            if (_currentState == GhoulState.Patrol || _currentState == GhoulState.Idle)
            {
                StartCoroutine(ScreamRoutine());
            }
        }
    }

    private bool HasLineOfSightToPlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 target = playerTransform.position + Vector3.up * 1.2f;

        if (Physics.Linecast(origin, target, out RaycastHit hit))
        {
            return (hit.transform == playerTransform);
        }
        return true; // Se não bateu em nada, assumimos que tem visão livre
    }
    #endregion

    #region Scream
    private IEnumerator ScreamRoutine()
    {
        _currentState = GhoulState.Screaming;

        // Parar movimento
        agent.SetDestination(transform.position);
        agent.velocity = Vector3.zero;

        // Toca animação
        if (view) view.PlayScreamAnimation();

        // Olha para o player
        LookAtPlayer();

        yield return new WaitForSeconds(model.screamDuration);

        EnterChaseState();
    }

    private void LookAtPlayer()
    {
        if (!playerTransform) return;
        Vector3 dir = (playerTransform.position - transform.position);
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }
    #endregion

    #region Chase
    private void EnterChaseState()
    {
        _currentState = GhoulState.Chasing;
        _chaseTimer = 0f;

        if (agent) agent.speed = model.runSpeed;
        if (view) view.PlayRunAnimation();
    }

    private void ChaseUpdate()
    {
        if (!playerTransform) return;

        agent.SetDestination(playerTransform.position);

        // Se perder linha de visão, inicia contagem
        if (!HasLineOfSightToPlayer())
        {
            _chaseTimer += Time.deltaTime;
            if (_chaseTimer >= model.chaseTimeout)
            {
                // Volta a patrulhar
                EnterPatrolState();
            }
        }
        else
        {
            _chaseTimer = 0f; // ainda vê o player
        }
    }
    #endregion

    #region Attack (Continuous Damage via AttackRange)
    /// <summary>
    /// Verifica se o player está dentro do attackRange e, se sim, inicia (ou mantém) a aplicação de dano.
    /// </summary>
    private void AttackCheck()
    {
        if (!playerTransform) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        // Se o player estiver dentro do attackRange e ainda não estivermos aplicando dano
        if (distanceToPlayer <= model.attackRange && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(ContinuousDamage());
        }
        // Se o player saiu do range e a coroutine estiver rodando, para-a
        else if (distanceToPlayer > model.attackRange && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine que, enquanto o player permanecer dentro do attackRange, dispara a animação de ataque,
    /// aguarda 1 segundo e aplica o dano, repetindo o ciclo a cada (attackCooldown) segundos.
    /// </summary>
    private IEnumerator ContinuousDamage()
    {
        while (Vector3.Distance(transform.position, playerTransform.position) <= model.attackRange)
        {
            // Dispara a animação de ataque (para feedback visual)
            if (view) view.PlayAttackAnimation();
            LookAtPlayer();

            // Aguarda 1 segundo antes de aplicar o dano
            yield return new WaitForSeconds(1f);

            // Aplica dano (se o player ainda estiver no range)
            if (Vector3.Distance(transform.position, playerTransform.position) <= model.attackRange)
            {
                var playerHealth = playerTransform.GetComponent<PlayerHealthController>();
                if (playerHealth != null)
                    playerHealth.TakeDamage((int)model.attackDamage);
            }

            // Aguarda o restante do cooldown (se attackCooldown for 2s, espera mais 1s)
            yield return new WaitForSeconds(model.attackCooldown - 1f);
        }
        damageCoroutine = null;
    }
    #endregion
}