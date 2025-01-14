using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera targetCamera;
    [SerializeField] private int priorityIncrease = 10;

    private int originalPriority;

    private void Start()
    {
        if (targetCamera != null)
        {
            originalPriority = targetCamera.Priority;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetCamera != null)
        {
            targetCamera.Priority += priorityIncrease;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && targetCamera != null)
        {
            targetCamera.Priority = originalPriority;
        }
    }
}
