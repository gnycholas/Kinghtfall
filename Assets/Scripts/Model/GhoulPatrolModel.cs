using UnityEngine;

[CreateAssetMenu(menuName = "Ghoul/GhoulPatrolModel")]
public class GhoulPatrolModel : ScriptableObject
{
    [Header("Configurações de movimento")]
    [Tooltip("Velocidade de caminhada do Ghoul.")]
    public float walkSpeed = 2f;

    [Header("Configurações de patrulha")]
    [Tooltip("Tempo de espera (em segundos) ao terminar de andar antes de escolher uma nova direção.")]
    public float idleTime = 2f;

    [Tooltip("Distância mínima do ponto aleatório (raio interno).")]
    public float minRandomDistance = 3f;

    [Tooltip("Distância máxima do ponto aleatório (raio externo).")]
    public float maxRandomDistance = 8f;

    [Header("Área de patrulha")]
    [Tooltip("Se quiser limitar a patrulha a uma área, defina um ponto central.")]
    public Transform patrolCenter;
}
