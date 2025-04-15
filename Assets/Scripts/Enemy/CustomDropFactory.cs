using System.Linq;
using UnityEngine;
using Zenject;

public class CustomDropFactory : IFactory<string, GameObject>
{
    [Inject] private GameObject[] _drops;
    [Inject] private DiContainer _container;
    public GameObject Create(string param)
    {
        var drop = GameObject.Instantiate(_drops.First(x => x.name == param));
        _container.InjectGameObject(drop.gameObject);
        return drop.gameObject;
    }
}