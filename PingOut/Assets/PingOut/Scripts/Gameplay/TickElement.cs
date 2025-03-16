using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class TickElement : MonoBehaviour
{
    public int historyTick = 0;
    public ElementPosition historyPos;
    public UnityAction<List<GameCommand>> onHistoryChange;

    public List<GameCommand> ElementHistory
    {
        get => elementHistory;
        set
        {
            elementHistory = value;
            DrawDebug();
        }
    }
    public List<GameCommand> elementHistory = new List<GameCommand>();

    public List<string> elementHistoryVisual = new List<string>();
    public ElementPosition currentPos = null;


    public GameTimeManager gameTime = null;

    protected virtual void Awake()
    {
        gameTime = GameTimeManager.Instance;
        GameTimeManager.OnTickChange += OnTickChange;
    }

    private void DrawDebug()
    {
        elementHistoryVisual = new List<string>(elementHistory.Count);
        for (int i = 0; i < elementHistory.Count; i++)
        {
            elementHistoryVisual.Add(elementHistory[i].DebugText());
        }

        onHistoryChange?.Invoke(elementHistory);
    }

    private void OnTickChange(int previousTick, int currentTick)
    {
        GameCommand orderToRefresh = null;

        if (previousTick == currentTick) return;
        if (previousTick < currentTick)
        {
            //Move Time Forward
            var incommingOrder = elementHistory.Where(x => x.startTick <= currentTick).OrderBy(x => x.startTick).ToList();
            if (incommingOrder.Count <= 0)
            {
                //Add waiting order
                RegisterOrder(new EmptyCommand(currentTick));
                return;
            }
            orderToRefresh = incommingOrder.Last();
        }
        else
        {
            // Move Time Backward
            var previousOrder = elementHistory.Where(x => x.startTick <= currentTick).OrderBy(x => x.startTick).ToList();
            if (previousOrder.Count <= 0)
            {
                Debug.Log("No order available" + this.name);
                return;
            }

            orderToRefresh = previousOrder.Last();
        }
        orderToRefresh.RefreshToTick(currentTick);
    }

    public void RegisterOrder(GameCommand newCommand)
    {
        ElementHistory.Add(newCommand);
        DrawDebug();
        historyTick += newCommand.Duration;
    }
    public void CancelLastOrder()
    {
        var lastCommand = elementHistory.Last();
        ElementHistory.Remove(lastCommand);
        DrawDebug();
        historyTick -= lastCommand.Duration;
    }
}
