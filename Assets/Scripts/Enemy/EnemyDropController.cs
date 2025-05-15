using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyDropController : MonoBehaviour
{
    [Inject] private DropFactory _dropFactory;
    [SerializeField] private Drop[] _drops;


    private void OnEnable()
    {
        var enemy = GetComponent<Enemy>();
        enemy.OnDie.AddListener(Drop);
    }

    private void OnDisable()
    {
        var enemy = GetComponent<Enemy>();
        enemy.OnDie.RemoveListener(Drop);
    }

    private void Drop()
    {
        var list = new List<string>();
        foreach (var drop in _drops)
        {
            for(int index = 0; index < drop.Percent; index++)
            {
                list.Add(drop.Name);
            }
        }
        var selectedDropName = list.OrderBy(x => Guid.NewGuid()).First();
        var gameObjec = _dropFactory.Create(selectedDropName);
        gameObjec.transform.position = transform.position;
    }
}
