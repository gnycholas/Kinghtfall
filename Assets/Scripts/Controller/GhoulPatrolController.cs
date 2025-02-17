using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum GhoulState
{
    Patrol,
    Idle,
    Screaming,
    Chasing,
    Attacking,
    Dead  // Novo estado para quando o ghoul morrer
}

public class GhoulPatrolController : MonoBehaviour
{
    [Header("MVC References")]
    [SerializeField] private GhoulPatrolModel model;
    public GhoulPatrolModel Model => model;

    [SerializeField] private GhoulPatrolView view;
    [SerializeField] private NavMeshAgent agent;

    [Header("Player")]
    [Tooltip("Arraste o Transform do jogador aqui.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Offset para o alvo de ataque (ex.: para atingir os pés do player).")]
    [SerializeField] private Vector3 attackTargetOffset = new Vector3(0, -1f, 0);

    private GhoulState _currentState = GhoulState.Patrol;
    private Vector3 _patrolCenter;
    private Coroutine _idleCoroutine;
    private float _chaseTimer;
    private Coroutine damageCoroutine;  // Corrotina que gerencia o ataque contínuo

    private void Start()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();

        // Define o centro de patrulha
        if (model && model.patrolCenter)
            _patrolCenter = model.patrolCenter.position;
        else
            _patrolCenter = transform.position;

        EnterPatrolState();
    }

    private void Update()
    {
        // Se o ghoul estiver morto, não processa nenhuma lógica adicional.
        if (_currentState == GhoulState.Dead)
            return;

        switch (_currentState)
        {
            case GhoulState.Patrol:
                PatrolUpdate();
                DetectPlayer();
                break;
            case GhoulState.Idle:
                DetectPlayer();
                break;
            case GhoulState.Screaming:
                // O ghoul está parado gritando
                break;
            case GhoulState.Chasing:
                ChaseUpdate();
                AttackCheck();
                break;
            case GhoulState.Attacking:
                // Durante o ataque, o movimento está bloqueado
                break;
        }
    }

    #region Patrol & Idle
    private void EnterPatrolState()
    {
        _currentState = GhoulState.Patrol;
        if (agent)
        {
            agent.speed = model.walkSpeed;
            agent.isStopped = false;
        }
        if (view) view.PlayWalkAnimation();
        ChooseNewDestination();
    }

    private void PatrolUpdate()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (_idleCoroutine == null)
                _idleCoroutine = StartCoroutine(IdleRoutine());
        }
    }

    private IEnumerator IdleRoutine()
    {
        _currentState = GhoulState.Idle;
        if (view) view.PlayIdleAnimation();
        yield return new WaitForSeconds(model.idleTime);
        _idleCoroutine = null;
        EnterPatrolState();
    }

    private void ChooseNewDestination()
    {
        float randomRadius = Random.Range(model.minRandomDistance, model.maxRandomDistance);
        Vector3 randomDir = Random.insideUnitSphere * randomRadius;
        randomDir += _patrolCenter;
        randomDir.y = _patrolCenter.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, model.maxRandomDistance, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            ChooseNewDestination();
    }
    #endregion

    #region Detection
    private void DetectPlayer()
    {
        if (!playerTransform || model == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > model.detectionRadius) return;

        if (model.fieldOfViewAngle > 0)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToPlayer);
            if (angle > model.fieldOfViewAngle * 0.5f)
                return;
        }

        if (HasLineOfSightToPlayer())
        {
            if (_currentState == GhoulState.Patrol || _currentState == GhoulState.Idle)
                StartCoroutine(ScreamRoutine());
        }
    }

    private bool HasLineOfSightToPlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 target = playerTransform.position + Vector3.up * 1.2f;
        if (Physics.Linecast(origin, target, out RaycastHit hit))
            return (hit.transform == playerTransform);
        return true;
    }
    #endregion

    #region Scream
    private IEnumerator ScreamRoutine()
    {
        _currentState = GhoulState.Screaming;
        agent.SetDestination(transform.position);
        agent.velocity = Vector3.zero;

        if (view) view.PlayScreamAnimation();
        LookAtPlayer();

        yield return new WaitForSeconds(model.screamDuration);
        EnterChaseState();
    }

    private void LookAtPlayer()
    {
        if (!playerTransform) return;
        Vector3 targetPos = playerTransform.position + attackTargetOffset;
        Vector3 direction = targetPos - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
    #endregion

    #region Chase
    private void EnterChaseState()
    {
        _currentState = GhoulState.Chasing;
        _chaseTimer = 0f;
        if (agent)
        {
            agent.speed = model.runSpeed;
            agent.isStopped = false;
        }
        if (view) view.PlayRunAnimation();
    }

    private void ChaseUpdate()
    {
        if (!playerTransform) return;
        agent.SetDestination(playerTransform.position + attackTargetOffset);

        if (!HasLineOfSightToPlayer())
        {
            _chaseTimer += Time.deltaTime;
            if (_chaseTimer >= model.chaseTimeout)
                EnterPatrolState();
        }
        else
        {
            _chaseTimer = 0f;
        }
    }
    #endregion

    #region Attack
    private void AttackCheck()
    {
        if (!playerTransform) return;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= model.attackRange && damageCoroutine == null)
            damageCoroutine = StartCoroutine(ContinuousAttack());
    }

    private IEnumerator ContinuousAttack()
    {
        _currentState = GhoulState.Attacking;
        agent.SetDestination(transform.position); // Bloqueia movimento durante o ataque

        // Completa o ciclo atual de ataque, mesmo que o player fuja durante a animação
        do
        {
            if (view) view.PlayAttackAnimation();
            LookAtPlayer();

            float attackAnimDuration = 1f; // Ajuste para a duração real da animação
            yield return new WaitForSeconds(attackAnimDuration);

            // Se o player ainda estiver no range, aplica dano
            if (Vector3.Distance(transform.position, playerTransform.position) <= model.attackRange)
            {
                PlayerController playerCtrl = playerTransform.GetComponent<PlayerController>();
                if (playerCtrl != null)
                    playerCtrl.TakeDamage((int)model.attackDamage);
            }

            yield return new WaitForSeconds(model.attackCooldown - attackAnimDuration);

        } while (Vector3.Distance(transform.position, playerTransform.position) <= model.attackRange);

        damageCoroutine = null;
        _currentState = GhoulState.Chasing;
    }
    #endregion

    #region Vida e Dano (Ghoul)
    public void TakeDamage(int damage)
    {
        model.currentHealth -= damage;
        if (model.currentHealth > 0)
        {
            if (view) view.TriggerHit();
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        _currentState = GhoulState.Dead;
        // Interrompe todas as corrotinas para que nenhuma ação extra seja executada
        StopAllCoroutines();
        if (agent)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        if (view) view.TriggerDie();
        // Opcional: Desative ou destrua o ghoul após um tempo
        // Destroy(gameObject, 3f);
    }
    #endregion

    #region Editor Gizmos
    private void OnDrawGizmosSelected()
    {
        if (model == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, model.attackRange);
    }
    #endregion
}
