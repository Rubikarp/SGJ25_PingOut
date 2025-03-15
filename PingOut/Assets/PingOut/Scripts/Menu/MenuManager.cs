using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject creditCanvas;
    public GameObject settingsCanvas;

    void Start()
    {
        // Désactive le canevas des crédits au démarrage
        if (creditCanvas != null)
        {
            creditCanvas.SetActive(false);
        }

        // Active le canevas du menu principal au démarrage
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(false);
        }
    }

    // Méthode appelée lorsque le bouton "crédit" est cliqué
    public void ShowCredit()
    {
        mainMenuCanvas.SetActive(false);
        creditCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    // Méthode appelée lorsque le bouton "retour" est cliqué
    public void ShowMainMenu()
    {
        creditCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void showSettingsMenu()
    {
        creditCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }
}