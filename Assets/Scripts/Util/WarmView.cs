using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class WarmView : MonoBehaviour
{
    public UnityEvent OnNear;
    public UnityEvent OnFarAway;
    private bool _visible;
    private bool _active;
    private CancellationTokenSource _cancellationWarmTokenSource;
    [Inject] private PlayerController _player;
    [Inject] private NotificationController _notification;
    [SerializeField] private string _warm;
    [SerializeField] private float _distance;
    [SerializeField] private float _delay;

    private void Start()
    {
        _active = true;
        CheckDistance().Forget();
    }
    private void OnDisable()
    {
        if(_cancellationWarmTokenSource != null)
        {
            _cancellationWarmTokenSource.Cancel(); 
            _cancellationWarmTokenSource.Dispose();
            _cancellationWarmTokenSource = null;
        } 
    }
    public void ToggleActive()
    {
        if (_active)
        {
            _active = false;
            _cancellationWarmTokenSource?.Cancel();
            _visible = false;
            _notification.HiddenNotification();
        }
        else
        {
            _active = true;
            CheckDistance().Forget();
        }
    }
    private async UniTaskVoid CheckDistance()
    {
        if(_cancellationWarmTokenSource != null)
        {
            _cancellationWarmTokenSource.Cancel();
        }
        _cancellationWarmTokenSource = new CancellationTokenSource();
        try
        {

            while (true)
            {
                var sqrDistance = (_player.transform.position - transform.position).sqrMagnitude;
                if (sqrDistance < _distance * _distance && !_visible)
                {
                    _visible = true;
                    _notification.ShowNotification(_warm);
                    OnNear?.Invoke();
                }
                else if (sqrDistance > _distance * _distance && _visible)
                {
                    _visible = false;
                    _notification.HiddenNotification();
                    OnFarAway?.Invoke();
                }
                await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: _cancellationWarmTokenSource.Token);
            }
        }
        catch (Exception ex) 
        { 
        }
        finally
        {
            if(_cancellationWarmTokenSource != null)
            { 
                _cancellationWarmTokenSource.Dispose();
                _cancellationWarmTokenSource = null;
            } 
        }
    }
}