using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MoveTutorialController : TutorialController
{
    private GameInputs _gameInputs;
    [SerializeField] private GameObject _moveWarmPrefab;

    private void Start()
    {
        _gameInputs = new GameInputs();
        _gameInputs.Enable(); 
    }
    private void OnDisable()
    {
        _gameInputs.Disable();
        _gameInputs.Dispose();
    }
    public async override void SetupTutorial()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        var warm = Instantiate(_moveWarmPrefab);
        TriggerTutorial(()=>_gameInputs.Gameplay.Move.IsPressed(), async () =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Destroy(warm.gameObject);
        }).Forget();
    }
}
