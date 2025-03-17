using System;
using UnityEngine;

public abstract class GameCommand
{
    public int startTime { get; set; } = 0;
    public int commandDuration { get; set; } = 1;
    public int EndTime => startTime + commandDuration;

    public abstract void RefreshToTick(int relativeTickTime);
    public abstract void RevertCommand();
    public abstract string DebugText();
    public abstract string DrawText();

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
        var baseDuration = 0;
        switch (type)
        {
            case EShootType.TopSpin:
                baseDuration = 1;
                break;
            case EShootType.Coupe:
                baseDuration = 3;
                break;
            case EShootType.Block:
                baseDuration = 2;
                break;
            case EShootType.Smash:
                baseDuration = 1;
                break;
            default:
                baseDuration = 3;
                break;
        }
        return baseDuration + 1;
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
    public override void RefreshToTick(int tickTime) { }
    public override void RevertCommand() { }
    public override string DebugText() => $"Wait order at {startTime}";
    public override string DrawText() => "Wait";


    public EmptyCommand(int startTick)
    {
        this.startTime = startTick;
        this.commandDuration = 1;
    }
}
public class SideFlipCommand : GameCommand
{
    public PlayerController playerRef;
    public bool switchingForRevers = true;
    public string GetWord() => switchingForRevers ? "Revers" : "Droit";

    public override void RefreshToTick(int tickTime)
    {
        if (tickTime < startTime || EndTime < tickTime)
        {
            //Nothing
        }
        else if (tickTime == startTime)
        {
            playerRef.IsInRevers = !switchingForRevers;
        }
        else if (tickTime == EndTime)
        {
            playerRef.IsInRevers = switchingForRevers;
        }
    }
    public override void RevertCommand()
    {

    }

    public override string DebugText() => $"Side flip order at {startTime}";
    public override string DrawText() => $"Mode {GetWord()}";

    public SideFlipCommand(int startTick, bool switchingForRevers, PlayerController playerRef)
    {
        this.startTime = startTick;
        this.commandDuration = 1;

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
        if (tickTime < startTime || EndTime < tickTime)
        {
            //Nothing
        }
        else if (tickTime == startTime)
        {
            playerRef.MoveToPos(beginPos);
        }
        else if (tickTime == EndTime)
        {
            playerRef.MoveToPos(finishPos);
        }
    }
    public override void RevertCommand()
    {
        playerRef.historyPos = beginPos;
    }
    public override string DebugText() => $"Move from {beginPos.name} to {finishPos.name} at {startTime}";
    public override string DrawText() => $"Move";

    public MovementCommand(int startTick, ElementPosition beginPos, ElementPosition endPos, PlayerController controller)
    {
        this.startTime = startTick;
        this.commandDuration = 1;

        this.beginPos = beginPos;
        this.finishPos = endPos;

        this.playerRef = controller;
    }
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
        if (tickTime < startTime)
        {
            //Nothing
        }
        if (EndTime + 1 < tickTime)
        {
            ballRef.OutAtPos(finishPos);
        }
        else if (tickTime == startTime)
        {
            ballRef.MoveToPos(beginPos);
            ballRef.shootType = type;
            AvantageManager.Instance.Addavantage(advantage);
        }
        else if (tickTime > startTime && tickTime <= EndTime)
        {
            float t = (float)(tickTime - startTime) / (float)(EndTime - startTime);
            var pos = Vector3.Lerp(beginPos.transform.position, finishPos.transform.position, t);
            ballRef.MoveToPos(pos);
            ballRef.shootType = type;
        }
        else if (tickTime == EndTime)
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
    public override string DebugText() => $"Shoot {Enum.GetName(typeof(EShootType), type)} to {finishPos.name} at {startTime}";
    public override string DrawText() => $"{Enum.GetName(typeof(EShootType), type)}";

    public ShootCommand(int startTick, EShootType shootType, int advantage, ElementPosition beginPos, ElementPosition endPos, BallController ballRef)
    {
        this.startTime = startTick;
        this.commandDuration = ShootDuration(shootType);

        this.beginPos = beginPos;
        this.finishPos = endPos;

        this.ballRef = ballRef;
        this.type = shootType;

        if (beginPos == ballRef.iaCenter || beginPos == ballRef.iaLeft || beginPos == ballRef.iaRight)
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
        if (tickTime == EndTime)
        {
            playerRef.TryShoot(this);
        }
    }
    public override void RevertCommand()
    {

    }

    public override string DebugText() => $"Prepare shoot of type {nameof(shootType)} at {startTime}";
    public override string DrawText() => $"{Enum.GetName(typeof(EShootType), shootType)}";

    public PrepareShootCommand(int startTick, EShootType shootType, PlayerController playerRef)
    {
        this.startTime = startTick;
        this.commandDuration = ShootPrepDuration(shootType);

        this.shootType = shootType;
        this.playerRef = playerRef;
    }
}
