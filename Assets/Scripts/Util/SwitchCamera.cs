using UnityEngine;
using Cinemachine;

public class SwitchCamera : MonoBehaviour
{
    private static CinemachineVirtualCamera _currentCamera;
    [SerializeField] private CinemachineVirtualCamera targetCamera;  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetCamera != null)
        {
            if(_currentCamera != null)
            {
                _currentCamera.Priority--;
            }
            _currentCamera = targetCamera;
            _currentCamera.Priority++;
        }
    } 
}
