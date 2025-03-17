using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallHistory : Singleton<BallHistory>
{
    [Header("Settings")]
    public BallState InitialState = new BallState();

    [Header("Position Data")]
    public ElementPosition[] PlayerSidePos => new ElementPosition[] { PlayerPosLeft, PlayerPosCenter, PlayerPosRight };
    public ElementPosition PlayerPosLeft;
    public ElementPosition PlayerPosCenter;
    public ElementPosition PlayerPosRight;
    public ElementPosition[] AdversaireSidePos => new ElementPosition[] { AdversairePosLeft, AdversairePosCenter, AdversairePosRight };
    public ElementPosition AdversairePosLeft;
    public ElementPosition AdversairePosCenter;
    public ElementPosition AdversairePosRight;

    [Header("History")]
    public List<BallCommand> History => history;
    public List<BallCommand> history = new List<BallCommand>(16);
    public int GetHistoryLenght => history.Select(x => x.duration).Sum();

    public BallState GetBallAtTime(int time)
    {
        var result = InitialState;
        time = Mathf.Max(GetHistoryLenght, time);

        int commandIndex = 0;
        int progressTime = 0;
        while (progressTime < time)
        {
            if (progressTime < history[commandIndex].EndTime)
            {
                result = history[commandIndex].ApplyCommand(result, progressTime);
                progressTime++;
            }
            else
            {
                commandIndex++;
            }
        }
        return result;
    }
    public BallCommand CommandAtTime(int time)
    {
        if (history.Count == 0) return null;

        time = Mathf.Max(GetHistoryLenght, time);

        int commandIndex = 0;
        int progressTime = 0;
        while (progressTime < time)
        {
            progressTime += history[commandIndex].duration;
            commandIndex++;
        }

        return history[commandIndex];
    }
}

public static class BallExtention
{
    public const int BASE_TRAVEL_DURATION = 1;
    public static int ShootDuration(this EShootType type)
    {
        switch (type)
        {
            case EShootType.TopSpin:
                return BASE_TRAVEL_DURATION + 1;
            case EShootType.Coupe:
                return BASE_TRAVEL_DURATION + 3;
            case EShootType.Block:
                return BASE_TRAVEL_DURATION + 2;
            case EShootType.Smash:
                return BASE_TRAVEL_DURATION + 1;
            default:
                return BASE_TRAVEL_DURATION + 3;
        }
    }
    public static int ShootPrepDuration(this EShootType type)
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

public enum EBallPos
{
    Left = 0,
    Center = 1,
    Right = 2,
}
public enum EShootType
{
    TopSpin = 0,
    Coupe = 1,
    Block = 2,
    Smash = 3,
}
[System.Serializable]
public class BallState
{
    public EBallPos originPos = EBallPos.Left;
    public int progressState = 0;
    public EBallPos aimingPos = EBallPos.Center;

    public bool isPlayerSide = false;
    public EShootType currentShoot = EShootType.Block;

    public int ComputeShootAdvantage(EShootType hitType)
    {
        switch (currentShoot)
        {
            case EShootType.TopSpin:
                switch (hitType)
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
                switch (hitType)
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
                switch (hitType)
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
                switch (hitType)
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
}

public class HitCommand : BallCommand
{
    public bool isPlayerShoot;
    public EBallPos beginPos;
    public EBallPos finishPos;

    public EShootType type;

    public override BallState ApplyCommand(BallState ballBeforeCommand, int time)
    {
        var newBallState = ballBeforeCommand;
        if (time < startTime)
        {
            return newBallState;
        }
        if (time == startTime)
        {
            newBallState.originPos = beginPos;
            newBallState.aimingPos = finishPos;
            newBallState.progressState = 0;
            newBallState.isPlayerSide = isPlayerShoot;
        }
        else if (startTime < time && time < EndTime)
        {
            newBallState.originPos = beginPos;
            newBallState.aimingPos = finishPos;
            newBallState.isPlayerSide = isPlayerShoot;
            newBallState.progressState = time - startTime;
        }
        else if (time >= EndTime)
        {
            newBallState.originPos = beginPos;
            newBallState.aimingPos = finishPos;
            newBallState.progressState = duration;
            newBallState.isPlayerSide = isPlayerShoot;
        }

        return newBallState;
    }

    public HitCommand (int startTime, bool isPlayerShoot, EBallPos beginPos, EBallPos finishPos, EShootType type)
    {
        this.startTime = startTime;
        this.type = type;
        this.duration = type.ShootDuration();

        this.isPlayerShoot = isPlayerShoot;
        this.beginPos = beginPos;
        this.finishPos = finishPos;
    }

    public override string CommandInfo() => $"{Enum.GetName(typeof(EShootType), type)} Hit";
    public override string CommandLog() => $"Shoot {Enum.GetName(typeof(EShootType), type)} " +
        $"from {Enum.GetName(typeof(EShootType), beginPos)} " +
        $"to {Enum.GetName(typeof(EShootType), finishPos)}";
}
public class ScoringCommand : BallCommand
{
    public bool isPlayerShoot;

    public override BallState ApplyCommand(BallState ballBeforeCommand, int time)
    {
        var newBallState = ballBeforeCommand;
        if (time < startTime)
        {
            return newBallState;
        }
        else if (startTime <= time)
        {
            newBallState.isPlayerSide = isPlayerShoot;
            newBallState.progressState = newBallState.currentShoot.ShootDuration() + 1;
        }

        return newBallState;
    }

    public ScoringCommand(int startTime, bool isPlayerShoot)
    {
        this.startTime = startTime;
        this.isPlayerShoot = isPlayerShoot;
    }

    public override string CommandInfo() => isPlayerShoot ? "Player Win" : "AI Win";
    public override string CommandLog() => isPlayerShoot ? $"Player Win at {startTime}" : $"Adversaire Win at {startTime}";
}