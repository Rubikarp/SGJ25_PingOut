using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class PlayerController : TickElement
{
    public ElementPosition[] playerPoses => new ElementPosition[] { posRR, posR, posL, posLL };
    public ElementPosition posRR;
    public ElementPosition posR;
    public ElementPosition posL;
    public ElementPosition posLL;

    [field: SerializeField]
    public int Test { get; private set; }


    public bool IsInRevers {
        get => isInRevers;
        set
        {
            isInRevers = value;
            transform.localScale = new Vector3(value ? -1 : 1, 1, 1);
        }
    }
    private bool isInRevers = false;


    public bool HistoryIsInRevers = false;
    protected override void Awake()
    {
        base.Awake();

        historyPos = currentPos;
        foreach (ElementPosition p in playerPoses)
        {
            p.OnTouchPos.AddListener(TryMoveToPos);
        }
    }

    [Button]
    public void ResgisterWaitMove()
    {
        var newOrder = new EmptyCommand(historyTick);
        RegisterOrder(newOrder);
    }

    public void PrepareShoot(int enumValue)
    {
        var newOrder = new PrepareShootCommand(historyTick, (EShootType)enumValue, this);
        RegisterOrder(newOrder);
    }
    public void TryMoveToPos(ElementPosition pos)
    {
        //Check les distances et si même position
        //TODO 

        if (pos == historyPos)
        {
            return;
        }
        int currentPosIndex = playerPoses.ToList().IndexOf(historyPos);
        int newPosIndex = playerPoses.ToList().IndexOf(pos);

        if (Mathf.Abs(currentPosIndex - newPosIndex) > 1)
        {
            int sign = (int)Mathf.Sign(newPosIndex - currentPosIndex);
            pos = playerPoses[currentPosIndex + sign];
        }

        var newOrder = new MovementCommand(historyTick, historyPos, pos, this);
        historyPos = pos;
        RegisterOrder(newOrder);
    }
    public void TryShoot(PrepareShootCommand shootPrep)
    {
        var ball = BallController.Instance;
        var lastBallCommand = ball.elementHistory.Last();
        if (lastBallCommand == null)
        {
            Debug.LogWarning("create fake shoot");
            lastBallCommand = new ShootCommand(historyTick, EShootType.TopSpin, 0, ball.iaCenter, ball.playerCenter, ball);
        }
        var shootData = ball.elementHistory.LastOrDefault(x => x is ShootCommand) as ShootCommand;
        var shootStartPos = shootData.finishPos;

        //TODO : check ball pos + revert ou droit
        bool touchBallInTime = shootPrep.endTick == historyTick;
        if (!touchBallInTime)
        {
            Debug.LogError($"Good in time start at {historyTick} but ball is here at {shootPrep.endTick}");
            return;
        }

        //Define if good position
        bool validPos = false;
        bool optimalPos = false;
        int posIndex = playerPoses.ToList().IndexOf(currentPos);

        //Define where to shoot
        var shootEndPos = ball.iaCenter;
        if (shootStartPos == ball.iaLeft)
        {
            shootEndPos = IsInRevers ? ball.playerCenter : ball.playerRight;
            validPos = posIndex == 0 || posIndex == 1;
            if (validPos)
            {
                optimalPos = IsInRevers ? posIndex == 0 : posIndex == 1;
            }
        }
        else
        if (shootStartPos == ball.iaCenter)
        {
            shootEndPos = IsInRevers ? ball.playerLeft : ball.playerRight;
            validPos = posIndex == 1 || posIndex == 2;
            if (validPos)
            {
                optimalPos = IsInRevers ? posIndex == 1 : posIndex == 2;
            }
        }
        else
        if (shootStartPos == ball.iaRight)
        {
            shootEndPos = IsInRevers ? ball.playerLeft : ball.playerCenter;
            validPos = posIndex == 2 || posIndex == 3;
            if (validPos)
            {
                optimalPos = IsInRevers ? posIndex == 2 : posIndex == 3;
            }
        }
        else
        if (shootStartPos == ball.playerLeft)
        {
            shootEndPos = IsInRevers ? ball.iaRight : ball.iaCenter;

            validPos = posIndex == 0 || posIndex == 1;
            if (validPos)
            {
                optimalPos = IsInRevers ? posIndex == 1 : posIndex == 0;
            }
        }
        else
        if (shootStartPos == ball.playerCenter)
        {
            shootEndPos = IsInRevers ? ball.iaRight : ball.iaLeft;
            validPos = posIndex == 1 || posIndex == 2;
            if (validPos)
            {
                optimalPos = IsInRevers ? posIndex == 2 : posIndex == 1;
            }
        }
        else
        if (shootStartPos == ball.playerRight)
        {
            shootEndPos = IsInRevers ? ball.iaCenter : ball.iaLeft;
            validPos = posIndex == 2 || posIndex == 3;
            if (validPos)
            {
                optimalPos = IsInRevers ? posIndex == 3 : posIndex == 2;
            }
        }

        int shootConfrontation = GameCommand.ShootConfrontation(ball.shootType, shootPrep.shootType);
        int avantage = shootConfrontation + (optimalPos ? 1 : 0);
        var newOrder = new ShootCommand(historyTick, shootPrep.shootType, avantage, shootStartPos, shootEndPos, ball);
        ball.RegisterOrder(newOrder);
    }
    public void FlipIsRevers()
    {
        var newOrder = new SideFlipCommand(historyTick, !HistoryIsInRevers, this);
        HistoryIsInRevers = !HistoryIsInRevers;
        RegisterOrder(newOrder);
    }

    public void MoveToPos(Vector3 pos) => transform.position = pos;
    public void MoveToPos(ElementPosition pos) => MoveToPos(pos.transform.position);

}
