using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorCamera : MonoBehaviour
{

    [SerializeField] private Transform jogadorTransform;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        transform.position = jogadorTransform.position + offset;
    }
}
