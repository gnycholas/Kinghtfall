using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChecarAreaDano : MonoBehaviour
{

    [SerializeField] private Image sanityBar;
    [SerializeField] private float sanity, maxSanity, damageCost, chargeRate;

    private bool isDamageZone;

    private Coroutine recharge;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isDamageZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isDamageZone = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDamageZone)
        {
            sanity -= damageCost * Time.deltaTime; // Consome a barra de sanidade...
            if (sanity < 0) sanity = 0;
            sanityBar.fillAmount = sanity / maxSanity;

            if (recharge != null) StopCoroutine(recharge); // Garante que a barra aguarde um segundo para que recarregue e que apenas uma corrotina seja executada por vez...
            recharge = StartCoroutine(rechargeSanity());
        }
    }

    private IEnumerator rechargeSanity()
    {
        yield return new WaitForSeconds(1f);

        while (!isDamageZone)
        {
            Debug.Log("Recuperando...");

            sanity += chargeRate / 5f;
            if (sanity > maxSanity) sanity = maxSanity;
            sanityBar.fillAmount = sanity / maxSanity;
            yield return new WaitForSeconds(.1f);
        }
    }
}
