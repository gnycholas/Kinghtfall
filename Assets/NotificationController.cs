using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageView;
     
    public async void ShowNoticationAsyn(string message, float time, NotificationParams param)
    {
        _messageView.text = message;
        await Task.Delay(TimeSpan.FromSeconds(time));
        HiddenNotification();
    }
    public void HiddenNotification()
    {
        _messageView.text = string.Empty;
    }

    public void ShowNotification(string message) 
    {
        ShowNoticationAsyn(message, 2, default);
    }
}
