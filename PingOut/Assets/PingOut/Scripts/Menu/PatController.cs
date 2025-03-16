using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PatController : MonoBehaviour
{
    public Image image; // L'image UI � animer
    public float popOutDuration = 0.1f; // Dur�e de l'animation de sortie rapide
    public float returnDuration = 0.5f; // Dur�e de l'animation de retour lente
    public float yOffset = 500f; // D�calage sur l'axe Y

    void Start()
    {
        // Ajouter un �couteur d'�v�nement au bouton
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

        // Calculer la nouvelle position de sortie avec un d�calage sur l'axe Y
        Vector3 popOutPosition = currentPosition + new Vector3(0, yOffset, 0);

        // Animer la sortie rapide de l'image
        image.transform.DOLocalMove(popOutPosition, popOutDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            // Animer le retour lent de l'image
            image.transform.DOLocalMove(currentPosition, returnDuration).SetEase(Ease.OutBounce);
        });
    }
}
