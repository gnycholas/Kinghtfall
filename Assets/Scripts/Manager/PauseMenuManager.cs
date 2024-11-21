using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    
    [SerializeField] private GameObject pausePanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    
    public void voltarParaPartida()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

}
