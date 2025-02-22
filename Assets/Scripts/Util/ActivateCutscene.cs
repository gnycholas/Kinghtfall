using UnityEngine;
using UnityEngine.Playables;

public class ActivateCutscene : MonoBehaviour
{

    [SerializeField] private PlayableDirector playableDirector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playableDirector.Play();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
