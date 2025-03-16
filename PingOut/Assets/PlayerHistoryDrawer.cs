using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHistoryDrawer : MonoBehaviour
{
    public PlayerController playerRef;

    public RectTransform historyPanel;
    public CommandDrawer commandDrawRefab;

    private void Awake()
    {
        playerRef.onHistoryChange += DrawHistory;
    }

    private void DrawHistory(List<GameCommand> history)
    {
        historyPanel.DeleteChildrens();
        foreach (var command in history)
        {
            var text = Instantiate(commandDrawRefab, historyPanel);
            text.DrawCommand(command);
        }

    }
}
