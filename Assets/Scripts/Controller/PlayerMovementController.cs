using System;
using ECM2;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private Character _character;
    private void Awake()
    {
        _character = GetComponent<Character>();
    }
    private void OnEnable()
    {
        GetComponent<PlayerController>().OnMove.AddListener(Move);
    }

    private void Move(Vector2 arg0)
    { 
        var fixedDirection = transform.TransformDirection(new Vector3(0, 0, arg0.y));
        _character.SetMovementDirection(fixedDirection);
        _character.AddYawInput(arg0.x);
    }

    private void OnDisable()
    {
        GetComponent<PlayerController>()?.OnMove.RemoveListener(Move);
    }
}
