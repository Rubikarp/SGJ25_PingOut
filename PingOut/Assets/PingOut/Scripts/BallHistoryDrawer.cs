using System.Collections.Generic;
using UnityEngine;

public class BallHistoryDrawer : MonoBehaviour
{
    public BallHistory ballRef;

    public RectTransform historyPanel;
    public CommandDrawer commandDrawRefab;

    private void Awake()
    {
        ballRef = BallHistory.Instance;
        ballRef.OnHistoryChange += DrawHistory;
    }

    private void DrawHistory(List<BallCommand> history)
    {
        historyPanel.DeleteChildrens();
        foreach (var command in history)
        {
            var text = Instantiate(commandDrawRefab, historyPanel);
            text.DrawCommand(command);
        }
    }
}
