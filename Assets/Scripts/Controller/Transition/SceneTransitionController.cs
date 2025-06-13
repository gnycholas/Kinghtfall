using System;
using Cysharp.Threading.Tasks; 
using UnityEngine.SceneManagement;
using Zenject;

public class SceneTransitionController
{  
    private async UniTask LoadSceneAsync(string scene)
    { 
        var sceneLoadTask = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single); 
        await sceneLoadTask;  
    }
    public async void LoadTransitionScene(SceneLoadParams sceneLoadParams, Action afterTransition = null, Action beforeTransition = null)
    {
        afterTransition?.Invoke();
        await LoadTransitionSceneAsync(sceneLoadParams,beforeTransition);
        beforeTransition?.Invoke(); 
    }

    private async UniTask LoadTransitionSceneAsync(SceneLoadParams transitionParams, Action beforeTransion)
    { 
        TransitionController.DoorIndex = transitionParams.DoorIndex;  
        await UniTask.Delay(TimeSpan.FromSeconds(transitionParams.Delay));
        await LoadSceneAsync("TransitionScene"); 
        await UniTask.WaitUntil(()=>TransitionController.IsCompleted); 
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
