using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class DoorController : MonoBehaviour,IInteract
{
    public UnityEvent OnOpenDoor;
    private Transform _door;
    [Inject] private GameplayController _gameplayController; 
    [SerializeField] private Requirement _requirement;
     
    private void Awake()
    {
        _door = transform.GetChild(0);
    }
    public async Task Execute()
    { 
        if (_requirement.Check(_gameplayController))
        { 
            await Open();
        }
    }

    private async Task Open()
    {
        _gameplayController.PlayerController.ToggleMove(false);
        await Task.Delay(TimeSpan.FromSeconds(2));
        _gameplayController.PlayerController.ToggleMove(true); 
        OnOpenDoor?.Invoke();
    }

    public Transform GetTarget()
    {
        return transform;
    } 
}
