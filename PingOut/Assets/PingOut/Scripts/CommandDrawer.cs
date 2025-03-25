using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandDrawer : MonoBehaviour
{
    public RectTransform timeStock;
    public Image timeUnit;
    [SerializeField] TextMeshProUGUI text;

    public void DrawCommand(AvatarCommand command) => DrawCommand(command.duration, command.CommandInfo());
    public void DrawCommand(BallCommand command) => DrawCommand(command.duration, command.CommandInfo());
    public void DrawCommand(int duration, string commandInfo)
    {
        timeStock.DeleteChildrens();
        for (int i = 0; i < duration; i++)
        {
            var newBlock = Instantiate(timeUnit, timeStock);
        }
        text.text = commandInfo;
    }
}
