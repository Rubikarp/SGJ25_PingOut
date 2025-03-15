using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public int historyTick = 0;
    public PlayerPosition historyPos;

    public List<TickAction> ballHistory = new List<TickAction>();
    public List<string> ballHistoryVisual = new List<string>();

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
        ballHistoryVisual = new List<string>(ballHistory.Count);
        for (int i = 0; i < ballHistory.Count; i++)
        {
            ballHistoryVisual.Add(ballHistory[i].DebugText());
        }
    }

    private void OnTickChange(int previousTick, int currentTick)
    {

        if (previousTick == currentTick) return;
        if (previousTick < currentTick)
        {
            //Move Time Forward
            var incommingOrder = ballHistory.Where(x => x.startTick <= currentTick).OrderBy(x => x.startTick).ToList();
            if (incommingOrder.Count <= 0)
            {
                Debug.Log("No order available");
                return;
            }

            incommingOrder.Last().RefreshToTick(currentTick);
        }
        else
        {
            // Move Time Backward
            var previousOrder = ballHistory.Where(x => x.startTick <= currentTick).OrderBy(x => x.startTick).ToList();
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
        ballHistory.Add(newOrder);
        historyTick += 1;
    }

    public void TryMoveToPos(PlayerPosition pos)
    {
        var newOrder = new OrderShoot(historyTick, 3, historyPos, pos, this);
        historyPos = pos;
        ballHistory.Add(newOrder);
        historyTick += 3;
    }

    public void MoveToPos(Vector3 pos) => transform.position = pos;

    public void MoveToPos(PlayerPosition pos) => MoveToPos(pos.transform.position);

    public class OrderShoot : TickAction
    {
        public PlayerPosition beginPos;
        public PlayerPosition finishPos;

        public BallController ballRef;

        public override void RefreshToTick(int tickTime)
        {
            if (tickTime < startTick)
            {

            }
            else if (tickTime == startTick)
            {
                ballRef.MoveToPos(beginPos);
            }
            else if (tickTime > startTick && tickTime <= endTick)
            {
                float t = (float)(tickTime - startTick) / (float)(endTick - startTick) ;
                var pos = Vector3.Lerp(beginPos.transform.position, finishPos.transform.position, t);
                ballRef.MoveToPos(pos);
            }
            else if (tickTime == endTick)
            {
                ballRef.MoveToPos(finishPos);
            }
            else
            if (tickTime > endTick)
            {

            }
        }

        public override string DebugText() => $"Move from {beginPos.name} to {finishPos.name} at {startTick}";

        public OrderShoot(int startTick, int tickDuration, PlayerPosition beginPos, PlayerPosition endPos, BallController ballRef)
        {
            this.startTick = startTick;
            this.endTick = startTick + tickDuration;

            this.beginPos = beginPos;
            this.finishPos = endPos;

            this.ballRef = ballRef;
        }
    }
}
