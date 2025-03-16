using UnityEngine;

public abstract class GameCommand
{
    public int startTick { get; set; }
    public int endTick { get; set; }
    public int Duration => endTick - startTick;

    public abstract void RefreshToTick(int relativeTickTime);
    public abstract void RevertCommand();
    public abstract string DebugText();

    public static int ShootConfrontation(EShootType incomming, EShootType response)
    {
        switch (incomming)
        {
            case EShootType.TopSpin:
                switch (incomming)
                {
                    case EShootType.TopSpin:
                        return 0;
                    case EShootType.Coupe:
                        return -1;
                    case EShootType.Block:
                        return 1;
                    case EShootType.Smash:
                        return 0;
                    default:
                        return 0;
                }
            case EShootType.Coupe:
                switch (incomming)
                {
                    case EShootType.TopSpin:
                        return 1;
                    case EShootType.Coupe:
                        return 0;
                    case EShootType.Block:
                        return -1;
                    case EShootType.Smash:
                        return 0;
                    default:
                        return 0;
                }
            case EShootType.Block:
                switch (incomming)
                {
                    case EShootType.TopSpin:
                        return 1;
                    case EShootType.Coupe:
                        return -1;
                    case EShootType.Block:
                        return 0;
                    case EShootType.Smash:
                        return 0;
                    default:
                        return 0;
                }
            case EShootType.Smash:
                switch (incomming)
                {
                    case EShootType.TopSpin:
                        return -10;
                    case EShootType.Coupe:
                        return -5;
                    case EShootType.Block:
                        return -1;
                    case EShootType.Smash:
                        return 0;
                    default:
                        return 0;
                }
            default:
                return 0;
        }

    }
    public static int ShootDuration(EShootType type)
    {
        switch (type)
        {
            case EShootType.TopSpin:
                return 1;
            case EShootType.Coupe:
                return 3;
            case EShootType.Block:
                return 2;
            case EShootType.Smash:
                return 1;
            default:
                return 3;
        }

    }
    public static int ShootPrepDuration(EShootType type)
    {
        switch (type)
        {
            case EShootType.TopSpin:
                return 3;
            case EShootType.Coupe:
                return 2;
            case EShootType.Block:
                return 1;
            case EShootType.Smash:
                return 1;
            default:
                return 3;
        }

    }
}
public class EmptyCommand : GameCommand
{
    public override void RefreshToTick(int tickTime){}
    public override void RevertCommand() {}
    public override string DebugText() => $"Wait order at {startTick}";


    public EmptyCommand(int startTick)
    {
        this.startTick = startTick;
        this.endTick = startTick + 1;
    }
}
public class SideFlipCommand : GameCommand
{
    public PlayerController playerRef;
    public bool switchingForRevers = true;

    public override void RefreshToTick(int tickTime)
    {
        if (tickTime < startTick || endTick < tickTime)
        {
            //Nothing
        }
        else if (tickTime == startTick)
        {
            playerRef.IsInRevers = !switchingForRevers;
        }
        else if (tickTime == endTick)
        {
            playerRef.IsInRevers = switchingForRevers;
        }
    }
    public override void RevertCommand()
    {

    }

    public override string DebugText() => $"Side flip order at {startTick}";

    public SideFlipCommand(int startTick, bool switchingForRevers, PlayerController playerRef)
    {
        this.startTick = startTick;
        this.endTick = startTick + 1;

        this.playerRef = playerRef;
        this.switchingForRevers = switchingForRevers;
    }
}
public class MovementCommand : GameCommand
{
    public ElementPosition beginPos;
    public ElementPosition finishPos;

    public PlayerController playerRef;

    public override void RefreshToTick(int tickTime)
    {
        if (tickTime < startTick || endTick < tickTime)
        {
            //Nothing
        }
        else if (tickTime == startTick)
        {
            playerRef.MoveToPos(beginPos);
        }
        else if (tickTime == endTick)
        {
            playerRef.MoveToPos(finishPos);
        }
    }
    public override void RevertCommand()
    {
        playerRef.historyPos = beginPos;
    }
    public override string DebugText() => $"Move from {beginPos.name} to {finishPos.name} at {startTick}";

    public MovementCommand(int startTick, ElementPosition beginPos, ElementPosition endPos, PlayerController controller)
    {
        this.startTick = startTick;
        this.endTick = startTick + 1;

        this.beginPos = beginPos;
        this.finishPos = endPos;

        this.playerRef = controller;
    }
}

public enum EShootType
{
    TopSpin = 0,
    Coupe = 1,
    Block = 2,
    Smash = 3,
}


public class ShootCommand : GameCommand
{
    public ElementPosition beginPos;
    public ElementPosition finishPos;

    public BallController ballRef;
    public EShootType type;
    public int advantage;

    public override void RefreshToTick(int tickTime)
    {
        if (tickTime < startTick)
        {
            //Nothing
        }
        if (endTick < tickTime)
        {
            ballRef.OutAtPos(finishPos);
        }
        else if (tickTime == startTick)
        {
            ballRef.MoveToPos(beginPos);
            ballRef.shootType = type;
            AvantageManager.Instance.Addavantage(advantage);
        }
        else if (tickTime > startTick && tickTime <= endTick)
        {
            float t = (float)(tickTime - startTick) / (float)(endTick - startTick);
            var pos = Vector3.Lerp(beginPos.transform.position, finishPos.transform.position, t);
            ballRef.MoveToPos(pos);
            ballRef.shootType = type;
        }
        else if (tickTime == endTick)
        {
            ballRef.MoveToPos(finishPos);
            ballRef.shootType = type;
        }
    }
    public override void RevertCommand()
    {
        ballRef.historyPos = beginPos;
        AvantageManager.Instance.Addavantage(-advantage);

    }
    public override string DebugText() => $"Move from {beginPos.name} to {finishPos.name} at {startTick}";

    public ShootCommand(int startTick, EShootType shootType, int advantage, ElementPosition beginPos, ElementPosition endPos, BallController ballRef)
    {
        this.startTick = startTick;
        this.endTick = startTick + ShootDuration(shootType);

        this.beginPos = beginPos;
        this.finishPos = endPos;

        this.ballRef = ballRef;
        this.type = shootType;

        if(beginPos == ballRef.iaCenter || beginPos == ballRef.iaLeft || beginPos == ballRef.iaRight)
        {
            advantage *= -1;
        }
        this.advantage = advantage;
    }
}
public class PrepareShootCommand : GameCommand
{
    public PlayerController playerRef;
    public EShootType shootType;

    public override void RefreshToTick(int tickTime)
    {
        if (tickTime == endTick - 1)
        {
            playerRef.TryShoot(this);
        }
    }
    public override void RevertCommand()
    {

    }

    public override string DebugText() => $"Prepare shoot of type {nameof(shootType)} at {startTick}";

    public PrepareShootCommand(int startTick, EShootType shootType, PlayerController playerRef)
    {
        this.startTick = startTick;
        this.endTick = startTick + ShootPrepDuration(shootType);

        this.shootType = shootType;
        this.playerRef = playerRef;
    }
}
