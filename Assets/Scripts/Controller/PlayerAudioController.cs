using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private bool _isRun;
    private float _timeReference;
    private float _currentTime;
    [SerializeField] private float _walkStepDelay;
    [SerializeField] private float _runStepDelay;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _clips;

    private void OnEnable()
    {
        GetComponent<PlayerController>().OnMove.AddListener(MoveAudio);
        GetComponent<PlayerController>().OnRun.AddListener(Run);
    }

    private void Run(bool arg0)
    {
        _isRun = arg0;
        _timeReference =(_isRun)? _runStepDelay:_walkStepDelay;
    }

    private void Update()
    {
        if (_currentTime <= _timeReference)
        {
            _currentTime += Time.deltaTime; 
        }
    }
    private void OnDisable()
    {
        GetComponent<PlayerController>().OnMove.RemoveListener(MoveAudio); 
    }

    private void MoveAudio(Vector2 arg0)
    {
        if (_source.isPlaying || _currentTime < _timeReference)
            return; 
        if(arg0.y > 0)
        {
            AudioClip selectedClip = default;
            _currentTime = 0;
            if (_isRun)
            {
                _timeReference = _runStepDelay;
                selectedClip = _clips[1];
            }
            else
            {
                _timeReference = _walkStepDelay;
                selectedClip = _clips[0];
            }
            _source.clip = selectedClip;
            _source.Play();
            Debug.Log("Play");
        } 
    }
}
