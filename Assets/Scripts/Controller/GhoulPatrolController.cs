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
    Hit,
    Dead
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

    [Tooltip("Offset para o alvo de ataque (ex.: para atingir os p�s do player).")]
    [SerializeField] private Vector3 attackTargetOffset = new Vector3(0, -1f, 0);

    [Header("Drop")]
    [Tooltip("Prefab da chave que ser� dropada quando o ghoul morrer.")]
    [SerializeField] private GameObject keyDropPrefab;

    private GhoulState _currentState = GhoulState.Patrol;
    private Vector3 _patrolCenter;
    private Coroutine _idleCoroutine;
    private float _chaseTimer;
    private Coroutine damageCoroutine;  // Corrotina que gerencia o ataque cont�nuo

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
        if (_currentState == GhoulState.Dead || _currentState == GhoulState.Hit)
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
                break;
            case GhoulState.Chasing:
                ChaseUpdate();
                AttackCheck();
                break;
            case GhoulState.Attacking:
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

        do
        {
            if (view) view.PlayAttackAnimation();
            LookAtPlayer();

            float attackAnimDuration = 1f; // Ajuste para a dura��o real da anima��o de ataque
            yield return new WaitForSeconds(attackAnimDuration);

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
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        model.currentHealth -= damage;
        if (model.currentHealth > 0)
        {
            StartCoroutine(HitRoutine());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator HitRoutine()
    {
        _currentState = GhoulState.Hit;
        if (agent)
        {
            agent.SetDestination(transform.position);
            agent.velocity = Vector3.zero;
        }
        if (view) view.TriggerHit();
        yield return new WaitForSeconds(1f); // Ajuste conforme a dura��o da anima��o de hit
        if (view) view.ResetHitAnimation();
        _currentState = GhoulState.Chasing;
    }

    private void Die()
    {
        _currentState = GhoulState.Dead;
        StopAllCoroutines();
        if (agent)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        if (view) view.TriggerDie();

        // Instancia o drop da chave, se o prefab estiver atribu�do
        if (keyDropPrefab != null)
        {
            Instantiate(keyDropPrefab, transform.position, Quaternion.identity);
        }
        // Opcional: destruir o ghoul ap�s algum tempo
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
