using System.Linq; 
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public int historyTick = 0;
    public PlayerPosition historyPos;

    public List<TickAction> playerHistory = new List<TickAction>();
    public List<string> playerHistoryVisual = new List<string>();

    public PlayerPosition[] playerPoses = null;
    public PlayerPosition currentPos = null;

    public GameTimeManager gameTime = null;


    private void Awake()
    {
        gameTime = GameTimeManager.Instance;
        GameTimeManager.OnTickChange += OnTickChange;

        historyPos = currentPos;
        foreach (PlayerPosition p in playerPoses)
        {
            p.OnTouchPos.AddListener(TryMoveToPos);
        }
    }

    [Button]
    private void OnValidate()
    {
        playerHistoryVisual = new List<string>(playerHistory.Count);
        for (int i = 0; i < playerHistory.Count; i++)
        {
            playerHistoryVisual.Add(playerHistory[i].DebugText());
        }
    }

    private void OnTickChange(int previousTick, int currentTick)
    {

        if (previousTick == currentTick) return;
        if(previousTick < currentTick)
        {
            //Move Time Forward
            var incommingOrder = playerHistory.Where(x => x.startTick >= previousTick).OrderBy(x => x.startTick).ToList();
            if (incommingOrder.Count <= 0)
            {
                Debug.Log("No order available");
                return;
            }

            incommingOrder.First().RefreshToTick(currentTick);
        }
        else
        {
            // Move Time Backward
            var previousOrder = playerHistory.Where(x => x.startTick <= currentTick).OrderBy(x => x.startTick).ToList();
            if (previousOrder.Count <= 0)
            {
                Debug.Log("No order available");
                return;
            }

            previousOrder.Last().RefreshToTick(currentTick);
        }

    }

    [Button] 
    public void ResgisterWaitMove()
    {
        var newOrder = new OrderEmpty(historyTick);
        playerHistory.Add(newOrder);
        historyTick += 1;
    }

    public void TryMoveToPos(PlayerPosition pos)
    {
        var newOrder = new OrderMovement(historyTick, historyPos, pos, this);
        historyPos = pos;
        playerHistory.Add(newOrder);
        historyTick += 1;
    }

    public void MoveToPos(PlayerPosition pos)
    {
        transform.position = pos.transform.position;
    }
}

public class OrderEmpty : TickAction
{
    public override void RefreshToTick(int tickTime)
    {
        Debug.Log("Wait Order");
    }

    public override string DebugText() => $"Wait order at {startTick}";

    public OrderEmpty(int startTick)
    {
        this.startTick = startTick;
        this.endTick = startTick + 1;
    }
}
public class OrderMovement : TickAction
{
    public PlayerPosition beginPos;
    public PlayerPosition finishPos;

    public PlayerController playerRef;

    public override void RefreshToTick(int tickTime)
    {
        if (tickTime < startTick)
        {

        }
        else if (tickTime == startTick)
        {
            playerRef.MoveToPos(beginPos);
        }
        else if (tickTime == endTick)
        {
            playerRef.MoveToPos(finishPos);
        }
        else 
        if (tickTime > endTick)
        {

        }
    }

    public override string DebugText() => $"Move from {beginPos.name} to {finishPos.name} at {startTick}";

    public OrderMovement(int startTick, PlayerPosition beginPos, PlayerPosition endPos, PlayerController controller)
    {
        this.startTick = startTick;
        this.endTick = startTick + 1;

        this.beginPos = beginPos;
        this.finishPos = endPos;

        this.playerRef = controller;
    }
}

public abstract class TickAction
{
    public int startTick { get; set; }
    public int endTick { get; set; }

    public abstract void RefreshToTick(int relativeTickTime);
    public abstract string DebugText();

}