using System; 
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

public class TransitionController : MonoBehaviour
{
    public static int DoorIndex;
    [Inject] private FadeController _fadeController;
    [SerializeField] private PlayableDirector[] _transition;

    public static bool IsCompleted { get; internal set; }

    private async void Start()
    {
        await OpenTheDoor();
    }

    private async UniTask OpenTheDoor()
    {
        await _fadeController.FadeIn(0.3f);
        var currentPlayarDirector = _transition[DoorIndex];
        currentPlayarDirector.gameObject.SetActive(true);
        currentPlayarDirector.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(currentPlayarDirector.duration));
        await _fadeController.FadeOut(0.3f);
        IsCompleted = true;
    }

    private void OnDestroy()
    {
        IsCompleted = false;
    }
}
