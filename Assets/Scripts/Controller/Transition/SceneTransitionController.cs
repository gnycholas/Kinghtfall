using System;
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
    public async void LoadTransitionScene(int doorIndex,SceneLoadParams sceneLoadParams, Action afterTransition = null, Action beforeTransition = null)
    {
        afterTransition?.Invoke();
        await LoadTransitionSceneAsync(doorIndex, sceneLoadParams,beforeTransition);
        beforeTransition?.Invoke(); 
    }

    private async UniTask LoadTransitionSceneAsync(int doorIndex, SceneLoadParams transitionParams, Action beforeTransion)
    { 
        TransitionController.DoorIndex = doorIndex;  
        await UniTask.Delay(TimeSpan.FromSeconds(transitionParams.Delay));
        await LoadSceneAsync("TransitionScene");
        await _fadeController.FadeIn(0.25f);
        await UniTask.WaitUntil(()=>TransitionController.IsCompleted);
        await _fadeController.FadeOut(0.25f);
        await LoadSceneAsync(transitionParams.Scene); 
        beforeTransion?.Invoke();
    }
}
[System.Serializable]
public class SceneLoadParams
{
    public string Scene;
    public int DoorIndex;
    public float Delay;
    public bool TransitionScene;
}
