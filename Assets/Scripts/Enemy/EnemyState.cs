using UnityEngine;
using UnityHFSM;

public abstract class EnemyState : MonoBehaviour
{
    public abstract State GetState();
}
