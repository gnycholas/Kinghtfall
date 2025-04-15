using System; 
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class PlayerAudioController : MonoBehaviour
{
    [Inject(Id = "Steps")] private AudioSource _stepSource;
    [Inject(Id = "PlayerVfx")] private AudioSource _vfxSource;
    [Inject(Id ="StepAudio")] private AudioClip[] _steps;
    [Inject(Id = "DamageAudio")] private AudioClip[] _takeDamage;
    [Inject(Id = "DieAudio")] private AudioClip _die;
    [Inject(Id = "RunAudio")] private AudioClip[] _stepsRun;
    [Inject(Id = "AttackAudio")] private AudioClip[] _attacks;
    [Inject] private PlayerController _playerController;
    [SerializeField] private float _stepTime;
    private bool _canPlayStepAudio = true;
    [SerializeField] private float _stepRunTime;
    private bool _canPlayStepRunAudio = true;

    public void OnEnable()
    {
        _playerController.OnPlayAudio.AddListener(PlayAudio);
    }
    private void OnDisable()
    {
        _playerController.OnPlayAudio.AddListener(PlayAudio);
    }

    public void PlayAudio(string name)
    {
        switch (name)
        {
            case "Walk": StepAudio(); break;
            case "Die": DyingAudio(); break;
            case "Run": StepRunAudio(); break;
            case "Attack": AttackSound(); break;
            case "TakeDamage":TakeDamageSound();break;
        }
    }

    private void TakeDamageSound()
    {
        _vfxSource.PlayOneShot(_takeDamage[UnityEngine.Random.Range(0, _takeDamage.Length)]);
        ProcessStepRunTime().Forget();
    }

    private void StepRunAudio()
    {
        if (!_canPlayStepRunAudio)
            return;
        _stepSource.PlayOneShot(_stepsRun[UnityEngine.Random.Range(0, _stepsRun.Length)]);
        ProcessStepRunTime().Forget();
    }
    private async UniTaskVoid ProcessStepRunTime()
    {
        _canPlayStepRunAudio = false;
        await UniTask.Delay(TimeSpan.FromSeconds(_stepRunTime));
        _canPlayStepRunAudio = true;
    }

    private void DyingAudio()
    {
        _vfxSource.PlayOneShot(_die);
    }

    private void StepAudio()
    {
        if (!_canPlayStepAudio)
            return;
        _stepSource.PlayOneShot(_steps[UnityEngine.Random.Range(0, _steps.Length)]);
        ProcessStepTime().Forget();
    }
    private async UniTaskVoid ProcessStepTime()
    {
        _canPlayStepAudio = false;
        await UniTask.Delay(TimeSpan.FromSeconds(_stepTime));
        _canPlayStepAudio = true;
    }

    private void AttackSound()
    {
        _vfxSource.PlayOneShot(_attacks[UnityEngine.Random.Range(0, _attacks.Length)]); 
    }
}
