using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BallController : TickElement
{
    public static BallController Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<BallController>();

            return instance;
        }
    }
    private static BallController instance;

    public UnityEvent onWin;
    public UnityEvent onLoose;

    public ElementPosition iaLeft;
    public ElementPosition iaCenter;
    public ElementPosition iaRight;
    public ElementPosition playerLeft;
    public ElementPosition playerCenter;
    public ElementPosition playerRight;

    public ElementPosition[] playerPoses => new ElementPosition[] { playerLeft, playerCenter, playerRight, iaLeft, iaCenter, iaRight };
    public EShootType shootType;

    protected override void Awake()
    {
        base.Awake();

        historyPos = currentPos;
        foreach (ElementPosition p in playerPoses)
        {
            p.OnTouchPos.AddListener(TryMoveToPos);
        }

        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        currentPos = iaLeft;
        historyPos = iaLeft;
        MoveToPos(iaLeft);

        TryMoveToPos(playerCenter);
    }
    protected virtual void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public void TryMoveToPos(ElementPosition pos)
    {
        var newOrder = new ShootCommand(historyTick, EShootType.Coupe, 0, historyPos, pos, this);
        historyPos = pos;
        RegisterOrder(newOrder);
    }

    public void MoveToPos(Vector3 pos) => transform.position = pos;
    public void MoveToPos(ElementPosition pos) => MoveToPos(pos.transform.position);
    public void OutAtPos(ElementPosition pos)
    {
        Debug.Log($"Ball Out at {pos.name}");

        if (pos == iaLeft || pos == iaCenter || pos == iaRight)
        {
            onWin?.Invoke();
        }
        else if (pos == playerLeft || pos == playerCenter || pos == playerRight)
        {
            onLoose?.Invoke();
        }
        else
        {
            Debug.LogError("Ball Out at unknown position");
        }
    }
}