using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class CustomUIFactory : IUIFactory
{
    [Inject] private UIRef[] _uiReferences;
    [Inject] private DiContainer _container;

    public GameObject Create(string param)
    {
        var selectedUI = _uiReferences.First(x => x.Name == param);
        var gameObject = Addressables.InstantiateAsync(selectedUI.Ref).WaitForCompletion();
        _container.InjectGameObject(gameObject);
        return gameObject;
    }
}
