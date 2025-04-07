using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class InGameUiView : MonoBehaviour
{
    [Header("Referências ao Painel de Inventário")]
    [SerializeField] private Transform itensGridPanel; // Arraste seu "ItensGridPanel" aqui no Inspector
    [SerializeField] private GameObject itemImagePrefab;// Este prefab pode ser só uma Image (UnityEngine.UI.Image) com layout configurado.

    // Para armazenar dinamicamente as imagens criadas
    private Dictionary<string, Image> itemImages = new Dictionary<string, Image>();

    [Header("Referências ao UI Model")]
    [SerializeField] private InGameUiModel uiModel;

    [Header("Referências aos Paineis de Tutorial")]
    public GameObject moveTutorial;
    public GameObject daggerTutorial;
    public GameObject potionTutorial;

    // Referente aos painéis de tutorial:
    public bool actionState; // Referência às bools de InGameUiModel, definida nos scripts de movimento ou coleta de itens.
    private GameObject tutorialPannel;

    /// <summary>
    /// Cria (ou reativa) a Image no grid. Ajusta o sprite e alpha.
    /// </summary>
    public void ShowItem(string itemKey, Sprite sprite, bool isEquipped)
    {
        Image img;
        // Verifica se já existe uma Image para esse item
        if (!itemImages.ContainsKey(itemKey))
        {
            // Instancia uma nova Image (ou outro objeto UI).
            GameObject newObj = Instantiate(itemImagePrefab, itensGridPanel);
            img = newObj.GetComponent<Image>();
            itemImages.Add(itemKey, img);
        }
        else
        {
            // Se já existe, reaproveita
            img = itemImages[itemKey];
        }

        // Ajusta o sprite e define o alpha (equipado ou não)
        img.sprite = sprite;
        float alpha = isEquipped ? 1f : 0.25f;
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);

        // Assegura que o objeto está ativo
        img.gameObject.SetActive(true);
    }

    /// <summary>
    /// Esconde (ou remove) o item do grid
    /// </summary>
    public void HideItem(string itemKey)
    {
        if (itemImages.ContainsKey(itemKey))
        {
            itemImages[itemKey].gameObject.SetActive(false);
            // Se quiser deletar permanentemente a instância (ao invés de só esconder):
            // Destroy(itemImages[itemKey].gameObject);
            // itemImages.Remove(itemKey);
        }
    }

    /// <summary>
    /// Mostra o painel de tutrial
    /// </summary>
    public void ShowTutorial(GameObject tutorialToShow)
    {
        tutorialPannel = tutorialToShow;

        //Ativando o painel de tutorial
        if (actionState == true)
        {
            tutorialToShow.SetActive(true);
            actionState = false;
            Debug.Log("Mostrou Painel");
        }
        StartCoroutine(DelayToRead(5f));
    }
    /// <summary>
    /// Esconde o painel de tutrial
    /// </summary>
    private void HideTutorial(GameObject tutorialToHide)
    {
        tutorialToHide.SetActive(false);
        Debug.Log("Escondeu Painel");
    }
    /// <summary>
    /// Delay para ler o tutrial
    /// </summary>
    public IEnumerator DelayToRead(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTutorial(tutorialPannel);
        Debug.Log("Leu Painel");

    }
}
