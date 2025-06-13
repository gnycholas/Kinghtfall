using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DaggerTutorialController : TutorialController,IEnvironmentState
{
    [SerializeField] private float _time;
    [SerializeField] private GameObject _warmPrefab;

    public int Hash => GetType().Name.GetHashCode();

    public void ChangeState(bool active)
    {
        if (active)
        {
            Destroy(this);
        }
    }

    public async override void SetupTutorial()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_time));
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        var warm = Instantiate(_warmPrefab);
        TriggerTutorial(async () => 
        {
            await UniTask.Delay(TimeSpan.FromSeconds(4));
            return true;
        }, async () =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            Destroy(warm.gameObject);
        }).Forget();
        GetComponent<EnviromentStateCheck>().ChangeState(Hash, true);
    }
}
