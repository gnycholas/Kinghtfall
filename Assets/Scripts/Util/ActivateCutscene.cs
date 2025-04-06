using UnityEngine;
using UnityEngine.Playables;

public class ActivateCutscene : MonoBehaviour
{
    private bool _firstTime = true;
    [SerializeField] private PlayableDirector playableDirector; 
    private void OnTriggerEnter(Collider other)
    {
        if (!_firstTime)
            return;
        if (other.CompareTag("Player"))
        {
            _firstTime = false;
            Active();
        }
    }

    public void Active()
    {
        playableDirector.Play();
        GetComponent<BoxCollider>().enabled = false;
    }
}
