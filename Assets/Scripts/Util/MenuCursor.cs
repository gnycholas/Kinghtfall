using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class MenuCursor : MonoBehaviour,
                          IPointerEnterHandler,
                          IPointerExitHandler,
                          IPointerClickHandler
{
    [Header("Ícone de Seleção (facultativo)")]
    [SerializeField] private GameObject selectionIcon;

    [Header("Sons")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private AudioSource audioSource;

    private void Awake()
    {
        // Pega (ou adiciona) o componente de áudio no mesmo GameObject
        audioSource = GetComponent<AudioSource>();

        // Se tiver um ícone de seleção, deixa ele desativado no início
        if (selectionIcon != null)
            selectionIcon.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectionIcon != null)
            selectionIcon.SetActive(true);

        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectionIcon != null)
            selectionIcon.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
