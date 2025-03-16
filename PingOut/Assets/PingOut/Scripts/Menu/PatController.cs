using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PatController : MonoBehaviour
{
    public Image image; // L'image UI à animer
    public float popOutDuration = 0.1f; // Durée de l'animation de sortie rapide
    public float returnDuration = 0.5f; // Durée de l'animation de retour lente
    public float yOffset = 500f; // Décalage sur l'axe Y

    void Start()
    {
        // Ajouter un écouteur d'événement au bouton
        if (image != null)
        {
            var button = image.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnImageClick);
            }
        }
    }

    void OnImageClick()
    {
        // Obtenir la position actuelle de l'image
        Vector3 currentPosition = image.transform.localPosition;

        // Calculer la nouvelle position de sortie avec un décalage sur l'axe Y
        Vector3 popOutPosition = currentPosition + new Vector3(0, yOffset, 0);

        // Animer la sortie rapide de l'image
        image.transform.DOLocalMove(popOutPosition, popOutDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            // Animer le retour lent de l'image
            image.transform.DOLocalMove(currentPosition, returnDuration).SetEase(Ease.OutBounce);
        });
    }
}
