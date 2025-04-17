using System; 
using Cysharp.Threading.Tasks;
using UnityEngine; 

public class TransitionController : MonoBehaviour
{
    public static int DoorIndex;
    [SerializeField] private GameObject _door;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AnimationCurve _curve;

    public static bool IsCompleted { get; internal set; }

    private async void Start()
    {
        await OpenTheDoor();
    }

    private async UniTask OpenTheDoor()
    { 
        var startPositionCamera = Camera.main.transform.position;
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f)); 
        for (float elapsed = 0; elapsed <= 1; elapsed += Time.deltaTime)
        {
            Camera.main.transform.position = Vector3.Lerp(startPositionCamera,
                startPositionCamera + Vector3.forward * 0.75f,
                elapsed);
            await UniTask.NextFrame();
        }
        _source.Play();
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));  
        for (float elapsed = 0; elapsed <= 1; elapsed += Time.deltaTime / 2.5f)
        {
            _door.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(Vector3.zero),
                Quaternion.Euler(0, 90, 0),
                _curve.Evaluate(elapsed));
            await UniTask.NextFrame();
        }
        IsCompleted = true;
    }
    private void OnDestroy()
    {
        IsCompleted = false;
    }
}
