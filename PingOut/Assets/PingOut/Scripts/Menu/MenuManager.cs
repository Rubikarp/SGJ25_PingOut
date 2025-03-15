using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject creditCanvas;
    public GameObject settingsCanvas;

    void Start()
    {
        // D�sactive le canevas des cr�dits au d�marrage
        if (creditCanvas != null)
        {
            creditCanvas.SetActive(false);
        }

        // Active le canevas du menu principal au d�marrage
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(false);
        }
    }

    // M�thode appel�e lorsque le bouton "cr�dit" est cliqu�
    public void ShowCredit()
    {
        mainMenuCanvas.SetActive(false);
        creditCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    // M�thode appel�e lorsque le bouton "retour" est cliqu�
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