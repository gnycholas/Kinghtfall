using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public async UniTask FadeIn(float fadeTime, float delay = 0)
    {
        await UniTask.Delay(delayTimeSpan:TimeSpan.FromSeconds(delay));
        for(float elapsed = 0; elapsed <= 1; elapsed+= Time.deltaTime / fadeTime)
        {
            _canvasGroup.alpha = 1 - elapsed;
            await UniTask.NextFrame();
        }
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    public async UniTask FadeOut(float fadeTime, float delay = 0)
    {
        await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(delay)); 
        _canvasGroup.blocksRaycasts = true;
        for (float elapsed = 0; elapsed <= 1; elapsed += Time.deltaTime / fadeTime)
        {
            _canvasGroup.alpha = elapsed;
            await UniTask.NextFrame();
        }
        _canvasGroup.alpha = 1;
    }
}
