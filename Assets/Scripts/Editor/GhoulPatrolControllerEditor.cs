using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Editor para desenhar gizmos de debug (raio de detecção, FOV e attackRange como arco)
/// </summary>
[CustomEditor(typeof(GhoulPatrolController))]
public class GhoulPatrolControllerEditor : Editor
{
    private void OnSceneGUI()
    {
        // "target" é o objeto que estamos inspecionando
        GhoulPatrolController controller = (GhoulPatrolController)target;
        if (controller == null) return;
        if (controller.Model == null) return; // caso seu script use: public GhoulPatrolModel Model => model;

        Vector3 centerPos = controller.transform.position;
        Vector3 forward = controller.transform.forward;

        // Desenha o raio de detecção (amarelo)
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(centerPos, Vector3.up, controller.Model.detectionRadius);

        // Desenha o FOV (arco ciano)
        if (controller.Model.fieldOfViewAngle > 0)
        {
            float halfFov = controller.Model.fieldOfViewAngle * 0.5f;
            Handles.color = Color.cyan;
            Handles.DrawWireArc(
                centerPos,
                Vector3.up,
                Quaternion.Euler(0, -halfFov, 0) * forward,
                controller.Model.fieldOfViewAngle,
                controller.Model.detectionRadius
            );

            Vector3 leftDir = Quaternion.Euler(0, -halfFov, 0) * forward * controller.Model.detectionRadius;
            Vector3 rightDir = Quaternion.Euler(0, halfFov, 0) * forward * controller.Model.detectionRadius;
            Handles.DrawLine(centerPos, centerPos + leftDir);
            Handles.DrawLine(centerPos, centerPos + rightDir);
        }

        // Desenha o attackRange como um arco (vermelho)
        if (controller.Model.fieldOfViewAngle > 0)
        {
            float halfFov = controller.Model.fieldOfViewAngle * 0.5f;
            Handles.color = Color.red;
            Handles.DrawWireArc(
                centerPos,
                Vector3.up,
                Quaternion.Euler(0, -halfFov, 0) * forward,
                controller.Model.fieldOfViewAngle,
                controller.Model.attackRange
            );

            // Opcional: desenha linhas para as extremidades do arco do attack range
            Vector3 leftAttackDir = Quaternion.Euler(0, -halfFov, 0) * forward * controller.Model.attackRange;
            Vector3 rightAttackDir = Quaternion.Euler(0, halfFov, 0) * forward * controller.Model.attackRange;
            Handles.DrawLine(centerPos, centerPos + leftAttackDir);
            Handles.DrawLine(centerPos, centerPos + rightAttackDir);
        }
        else
        {
            // Caso o fieldOfViewAngle seja 0, desenha um círculo (fallback)
            Handles.color = Color.red;
            Handles.DrawWireDisc(centerPos, Vector3.up, controller.Model.attackRange);
        }
    }
}
