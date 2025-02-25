using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    [Header("Refer�ncia aos objetos")]
    [SerializeField] private PlayerModel playerModel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject stageFinish;
    
    [Header("Refer�ncia �s telas")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameCompletePanel;

    public float interactionRadius = 2f;

    // Start is called before the first frame update
    void Start()
    {
        if (playerModel == null || gameOverPanel == null)
        {
            Debug.Log("Player model ou Game over panel n�o identificado no Game state controller. O fluxo de telas pode n�o funcionar...");
        }
    }

    // Update is called once per frame
    void Update()
    {
        int playerLife = playerModel.currentHealth;

        if (playerLife <= 0)
        {
            gameOverPanel.SetActive(true);
        }

        CheckGameFinish(stageFinish);
    }

    private void CheckGameFinish(GameObject interactible)
    {
        if (interactible == null) return;
        if (!interactible.activeInHierarchy) return;

        float distance = Vector3.Distance(player.transform.position, interactible.transform.position);

        if (distance <= interactionRadius)
        {
            gameCompletePanel.SetActive(true);
        }
    }
}
