using System;
using UnityEngine; 
using Zenject;

public class EnviromentStateCheck : MonoBehaviour
{ 
    [Inject] private SaveManager _saveManager; 

    private void Awake()
    {
        foreach (var environmentState in GetComponents<IEnvironmentState>())
        {
            _saveManager.CheckState(environmentState); 
        }
    }


    public void ChangeState(int hash, bool state)
    {
        _saveManager.ChangeState(hash, state);
    }
}

public interface IEnvironmentState
{
    public int Hash { get; } 
    public void ChangeState(bool active);
}