using UnityEngine;
using UnityEditor;

[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
public static class CollectibleItemGizmos
{
    public static void DrawGizmoForCollectible(CollectibleItem item, GizmoType gizmoType)
    {
        if (item == null)
            return;

        // Define uma cor (por exemplo, amarelo) para desenhar o Gizmo
        Gizmos.color = Color.yellow;
        // Desenha uma esfera wireframe na posição do item com o raio definido
        Gizmos.DrawWireSphere(item.transform.position, item.collectionRadius);
    }
}
