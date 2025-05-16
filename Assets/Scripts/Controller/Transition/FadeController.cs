using Cysharp.Threading.Tasks;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public async UniTask FadeIn(float fadeTime)
    {
        for(float elapsed = 0; elapsed <= 1; elapsed+= Time.deltaTime / fadeTime)
        {
            _canvasGroup.alpha = 1 - elapsed;
            await UniTask.NextFrame();
        }
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    public async UniTask FadeOut(float fadeTime)
    {
        _canvasGroup.blocksRaycasts = true;
        for (float elapsed = 0; elapsed <= 1; elapsed += Time.deltaTime / fadeTime)
        {
            _canvasGroup.alpha = elapsed;
            await UniTask.NextFrame();
        }
        _canvasGroup.alpha = 1;
    }
}
