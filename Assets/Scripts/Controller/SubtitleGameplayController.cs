using UnityEngine;
using TMPro;

public class SubtitleGameplayController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textView;

    public void Hidden()
    {
        _textView.text = string.Empty;
    }

    public void ShowText(string message)
    {
        _textView.text =message;
    }
}
