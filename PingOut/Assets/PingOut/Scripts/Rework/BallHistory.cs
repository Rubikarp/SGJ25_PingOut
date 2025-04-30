using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BallHistory : Singleton<BallHistory>
{
    public UnityAction<List<BallCommand>> OnHistoryChange { get => onHistoryChange; set => onHistoryChange = value; }
    public UnityAction<List<BallCommand>> onHistoryChange;

    [Header("Settings")]
    public BallState InitialState = new BallState();
    public HitCommand InitialCommand = new HitCommand(0, false, EBallPos.Left, EBallPos.Center, EShootType.Coupe);

    public PlayerHistory AvatarHistory => PlayerHistory.Instance;
    public AdversaireHistory AdversaireHistory => AdversaireHistory.Instance;

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
        if (GetHistoryLenght <= time) return history.Last();

        int progressTime = 0;
        int commandIndex = 0;
        for (commandIndex = 0; commandIndex < history.Count; commandIndex++)
        {
            if (progressTime < time)
            {
                progressTime += history[commandIndex].duration;
                if (commandIndex < history.Count - 1) 
                {
                    return history[commandIndex];
                }
            }
            else
            {
                return history[commandIndex];
            }
        }
        return history[commandIndex];
    }

    protected override void Awake()
    {
        base.Awake();

        PlayerHistory.Instance.OnHistoryChange += OnPlayerHistoryChange;
        AdversaireHistory.Instance.onHistoryChange += OnEnnemyHistoryChange;
    }

    private void Start()
    {
        RecalculateHistory();
    }

    private void OnEnnemyHistoryChange(List<AvatarCommand> arg0) => RecalculateHistory();
    private void OnPlayerHistoryChange(List<AvatarCommand> arg0) => RecalculateHistory();
    [Button]
    private void RecalculateHistory()
    {
        var player = PlayerHistory.Instance;
        var ennemy = AdversaireHistory.Instance;

        //clear ball history
        History.Clear();
        History.Add(InitialCommand);

        var maxDuration = Mathf.Max(player.GetHistoryLenght, ennemy.GetHistoryLenght, GetHistoryLenght);
        for (int time = 0; time < maxDuration; time++)
        {
            //Check if there is a command at this time
            if (CommandAtTime(time) != null) continue;

            HitCommand previousBallCommand = History.FindLast(command => command is HitCommand) as HitCommand;

            var ballState = GetBallAtTime(time);
            var playerState = player.GetAvatarAtTime(time);
            var ennemyState = ennemy.GetAvatarAtTime(time);

            if (ballState.isPlayerSide)
            {
                var ennemyCommand = ennemy.GetCommandAtTime(time);

                if (ennemyCommand != null || ennemyCommand is not ActionCommand)
                {
                    var newCommand = new ScoringCommand(time, ballState.isPlayerSide);
                    History.Add(newCommand);
                    return;
                }
                else 
                {
                    ActionCommand shootCommand = ennemyCommand as ActionCommand;
                    if (shootCommand.EndTime == time)
                    {
                        //TODO : Add HitCommand

                        EBallPos? aimingPos = ComputeShootDir(previousBallCommand, playerState.currentPos, playerState.isInReversMode);

                        var newCommand = new HitCommand(time, !ballState.isPlayerSide, previousBallCommand.finishPos, aimingPos, shootCommand.type);
                        History.Add(newCommand);
                    }
                    else
                    {
                        var newCommand = new ScoringCommand(time, ballState.isPlayerSide);
                        History.Add(newCommand);
                        return;
                    }
                }
            }
            else
            {
                var playerCommand = player.GetCommandAtTime(time);
                if (playerCommand != null || playerCommand is not ActionCommand)
                {
                    var newCommand = new ScoringCommand(time, ballState.isPlayerSide);
                    History.Add(newCommand);
                    return;
                }
                else
                {
                    ActionCommand shootCommand = playerCommand as ActionCommand;
                    if (shootCommand.EndTime == time)
                    {
                        //TODO : Add HitCommand

                        //var newCommand = new HitCommand(time, !ballState.isPlayerSide, ballState.aimingPos, shootCommand.aimingPos, shootCommand.type);
                        //History.Add(newCommand);
                    }
                    else
                    {
                        var newCommand = new ScoringCommand(time, ballState.isPlayerSide);
                        History.Add(newCommand);
                        return;
                    }
                }
            }

            OnHistoryChange?.Invoke(History);
        }
    }

    private EBallPos? ComputeShootDir(HitCommand previousHit, EAvatarPos currentPos, bool isInReversMode)
    {
        int ballIndex = (int)currentPos;
        int userIndex = (int)previousHit.finishPos;

        //If the ball is near the player
        int ballDistance = ballIndex - userIndex;
        ballDistance -= Convert.ToInt32(isInReversMode);

        //check if miss the ball
        if (ballDistance != 0) return null;

        int sendPos = (int)previousHit.finishPos;
        sendPos += isInReversMode ? 1 : -1;
        sendPos = Mathf.Clamp(sendPos, 0, 2);

        return (EBallPos)sendPos;
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

    public override string CommandInfo() => $"{Enum.GetName(typeof(EShootType), type)}";
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