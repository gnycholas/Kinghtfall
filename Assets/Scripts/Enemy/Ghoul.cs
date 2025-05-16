using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public sealed class Ghoul : Enemy
{
    [SerializeField] private AssetReference _audioScreamRef;
    [SerializeField] private AudioSource _audioSource;
    public void Screaming()
    {
        _audioSource.PlayOneShot(Addressables.LoadAssetAsync<AudioClip>(_audioScreamRef).WaitForCompletion());
        Agent.enabled = false;
        transform.LookAt(Target, Vector3.up);
        Agent.enabled = true;
        Agent.isStopped = true;
        Play("GhoulScream");
        target = player.transform;
        Agent.SetDestination(target.position);
    }
}
