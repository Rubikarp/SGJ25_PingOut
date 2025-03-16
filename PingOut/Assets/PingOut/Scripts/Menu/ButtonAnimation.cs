using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button; // Le bouton à animer
    public float scaleDuration = 0.2f; // Durée de l'animation de scale
    public Vector3 targetScale = new Vector3(0.9f, 0.9f, 0.9f); // Échelle cible lors du survol
    private Vector3 originalScale; // Échelle originale du bouton

    void Start()
    {
        // Sauvegarder l'échelle originale du bouton
        originalScale = button.transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Animer l'échelle du bouton lorsque le pointeur entre
        button.transform.DOScale(targetScale, scaleDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Restaurer l'échelle originale du bouton lorsque le pointeur sort
        button.transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);
    }
}
