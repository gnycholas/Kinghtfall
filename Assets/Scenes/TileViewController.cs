using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileViewController : MonoBehaviour, IBeginDragHandler,IEndDragHandler,IDragHandler
{
    [SerializeField] private RectTransform _myRecttranform;
    [SerializeField] private float _speed;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin");
    }

    public void OnDrag(PointerEventData eventData)
    {
        _myRecttranform.anchoredPosition += Input.mousePosition.magnitude;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End");
    }
}
