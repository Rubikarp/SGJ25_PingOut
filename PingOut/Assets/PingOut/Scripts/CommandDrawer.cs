using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandDrawer : MonoBehaviour
{
    public RectTransform timeStock;
    public Image timeUnit;
    [SerializeField] TextMeshProUGUI text;

    public void DrawCommand(GameCommand command)
    {
        timeStock.DeleteChildrens();
        for(int i = 0; i < command.commandDuration; i++)
        {
            var newBlock = Instantiate(timeUnit, timeStock);
        }
        text.text = command.DrawText();
    }
}
