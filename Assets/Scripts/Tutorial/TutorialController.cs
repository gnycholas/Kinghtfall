using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class TutorialController : MonoBehaviour
{
    protected async UniTaskVoid TriggerTutorial(Func<bool> waitFor, Action onCompleted)
    {
        await UniTask.WaitUntil(waitFor);
        onCompleted?.Invoke();
    }
    protected async UniTaskVoid TriggerTutorial(Func<UniTask<bool>> waitFor, Action onCompleted)
    {
        await waitFor.Invoke();
        onCompleted?.Invoke();
    }

    public abstract void SetupTutorial();
}
