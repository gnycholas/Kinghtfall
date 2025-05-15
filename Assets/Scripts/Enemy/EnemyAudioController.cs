using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EnemyAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AssetReference _hitAudioRef;
    [SerializeField] private AssetReference _dieAudioRef;

    private void OnEnable()
    {
        var mediator = GetComponent<Enemy>();
        mediator.OnTakeDamage.AddListener(HitAudio);
        mediator.OnDie.AddListener(DieAudio);
    }


    private void OnDisable()
    {
        var mediator = GetComponent<Enemy>();
        mediator.OnTakeDamage.RemoveListener(HitAudio);
        mediator.OnDie.RemoveListener(DieAudio);
    }

    private void HitAudio(DamageInfo info)
    {
        if(info.Damage > 0)
        {
            _audioSource.PlayOneShot(Addressables.LoadAssetAsync<AudioClip>(_hitAudioRef).WaitForCompletion());
        }
    }

    private void DieAudio()
    {
        _audioSource.PlayOneShot(Addressables.LoadAssetAsync<AudioClip>(_dieAudioRef).WaitForCompletion());
    }
}
