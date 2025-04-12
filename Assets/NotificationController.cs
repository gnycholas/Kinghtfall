using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageView;
     
    public void ShowNotification(string message, NotificationParams param)
    {
        _messageView.text = message; 
    }
    public void HiddenNotification()
    {
        _messageView.text = string.Empty;
    }

    public void ShowNotification(string message) 
    {
        ShowNotification(message, default);
    }
}
