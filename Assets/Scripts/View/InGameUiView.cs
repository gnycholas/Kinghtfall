using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InGameUiView : MonoBehaviour
{
    [Header("Referências ao Painel de Inventário")]
    [SerializeField] private Transform itensGridPanel; // Arraste seu "ItensGridPanel" aqui no Inspector
    [SerializeField] private GameObject itemImagePrefab;
    // Este prefab pode ser só uma Image (UnityEngine.UI.Image) com layout configurado.

    // Para armazenar dinamicamente as imagens criadas
    private Dictionary<string, Image> itemImages = new Dictionary<string, Image>();

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
}
