using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineVirtualCamera cameraParaAtivar;
    public CinemachineVirtualCamera cameraParaDesativar;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cameraParaAtivar.Priority = 15;
            cameraParaDesativar.Priority = 5;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cameraParaAtivar.Priority = 5;
            cameraParaDesativar.Priority = 15;
        }
    }
}
