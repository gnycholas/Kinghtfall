using Cysharp.Threading.Tasks; 
using UnityEngine.SceneManagement;
using Zenject;

public class SceneTransitionController
{ 
    [Inject] private FadeController _fadeController;
    
    private async UniTask LoadSceneAsync(string scene)
    {
        var sceneLoadTask = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single); 
        await sceneLoadTask; 
    }
    public async void LoadTransitionScene(int doorIndex,SceneLoadParams sceneLoadParams)
    {
        await LoadTransitionSceneAsync(doorIndex, sceneLoadParams);
    }

    private async UniTask LoadTransitionSceneAsync(int doorIndex, SceneLoadParams transitionParams)
    {
        TransitionController.DoorIndex = doorIndex; 
        await _fadeController.FadeOut(transitionParams.FadeTime);
        await LoadSceneAsync("TransitionScene");
        await _fadeController.FadeIn(transitionParams.FadeTime);
        await UniTask.WaitUntil(()=>TransitionController.IsCompleted);
        await UniTask.WhenAll(
        _fadeController.FadeOut(transitionParams.FadeTime),
        LoadSceneAsync(transitionParams.Scene));
    }
}
[System.Serializable]
public class SceneLoadParams
{
    public string Scene;
    public int DoorIndex;
    public float FadeTime;
    public bool TransitionScene;
}
