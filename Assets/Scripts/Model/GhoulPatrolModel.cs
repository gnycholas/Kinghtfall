using UnityEngine;

[CreateAssetMenu(menuName = "Ghoul/GhoulPatrolModel")]
public class GhoulPatrolModel : ScriptableObject
{
    [Header("Patrulha")]
    public float walkSpeed = 2f;
    public float idleTime = 2f;
    public float minRandomDistance = 3f;
    public float maxRandomDistance = 8f;
    public Transform patrolCenter;

    [Header("Deteccao do jogador")]
    public float detectionRadius = 10f;
    public float fieldOfViewAngle = 120f;
    public float screamDuration = 2f;

    [Header("Chase (Perseguicao)")]
    public float runSpeed = 4f;
    public float chaseTimeout = 5f;

    [Header("Ataque")]
    public float attackRange = 1.5f;
    public float attackDamage = 1f;        // Se quiser causar dano
    public float attackCooldown = 2f;       // Ex: tempo entre ataques
}