using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class WarmView : MonoBehaviour
{
    public UnityEvent OnNear;
    public UnityEvent OnFarAway;
    private bool _visible;
    [Inject] private PlayerController _player;
    [Inject] private NotificationController _notification;
    [SerializeField] private string _warm;
    [SerializeField] private float _distance;
    [SerializeField] private float _delay;

    private void Start()
    {
        CheckDistance().Forget();
    }

    private async UniTaskVoid CheckDistance()
    {
        while (true)
        {
            var sqrDistance = (_player.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < _distance * _distance && !_visible)
            {
                _visible = true;
                _notification.ShowNotification(_warm);
                OnNear?.Invoke();
            }else if(sqrDistance > _distance * _distance && _visible)
            {
                _visible = false;
                _notification.HiddenNotification();
                OnFarAway?.Invoke(); 
            }
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
        }
    }
}