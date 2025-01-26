using UnityEngine;

[CreateAssetMenu(menuName = "Ghoul/GhoulPatrolModel")]
public class GhoulPatrolModel : ScriptableObject
{
    [Header("Configurações de movimento - Patrulha")]
    public float walkSpeed = 2f;
    public float idleTime = 2f;
    public float minRandomDistance = 3f;
    public float maxRandomDistance = 8f;
    public Transform patrolCenter;

    [Header("Detecção do jogador")]
    [Tooltip("Raio de visão do monstro (em metros).")]
    public float detectionRadius = 10f;

    [Tooltip("Se quiser simular campo de visão, defina ângulo (em graus). 0 = desativado.")]
    public float fieldOfViewAngle = 120f;

    [Header("Animação de scream")]
    [Tooltip("Duração (em segundos) da animação de scream antes de iniciar corrida.")]
    public float screamDuration = 2f;

    [Header("Configurações de corrida (chase)")]
    public float runSpeed = 4f;
    [Tooltip("Após perder a visão do jogador, quanto tempo até retomar patrulha.")]
    public float chaseTimeout = 5f;
}
