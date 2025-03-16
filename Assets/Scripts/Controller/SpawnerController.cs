using System;
using UnityEngine;
using Zenject;

public class SpawnerController : MonoBehaviour
{
    public Action<PlayerController> OnSpawn;
    public Action<PlayerController> OnDespawn;
    private PlayerController _player;
    [Inject(Id = "Player")] private GameObject _prefabPlayerController;
    [SerializeField] private bool _spawnOnAwake;
    [SerializeField] private Transform _spawnPoint;

    public PlayerController Player { get { return _player; } }

    private void Awake()
    {
        if (_spawnOnAwake)
        {
            SpawnPlayer(_spawnPoint.position,_spawnPoint.rotation);
        }
    }

    public void SpawnPlayer(Vector3 position, Quaternion rotation)
    {
        _player = Instantiate(_prefabPlayerController, position, rotation).GetComponent<PlayerController>();
        OnSpawn?.Invoke(_player);
        OnDespawn?.Invoke(_player);
    }
}
