using UnityEngine;
using System;

public class CollectiblesController : MonoBehaviour
{
    [SerializeField] private ItemInfo _item; 
    private PlayerController _playerController;
    public float collectionRadius = 2f;

    private void Awake()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
    }
     
    private void ShowCollectionMessage(string message)
    {
        FindAnyObjectByType<SubtitleGameplayController>()?.ShowText(message);
    }

    private void HideCollectionMessage()
    {
        FindAnyObjectByType<SubtitleGameplayController>()?.Hidden();

    }
}
