using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DaggerTutorialController : TutorialController
{
    [SerializeField] private float _time;
    [SerializeField] private GameObject _moveWarmPrefab;
     
    public async override void SetupTutorial()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_time));
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        var warm = Instantiate(_moveWarmPrefab);
        TriggerTutorial(async () => 
        {
            await UniTask.Delay(TimeSpan.FromSeconds(4));
            return true;
        }, async () =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Destroy(warm.gameObject);
        }).Forget();
    }
}
