using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvatarHistory : MonoBehaviour
{
    [Header("Settings")]
    public AvatarState InitialState = new AvatarState();

    [Header("Position Data")]
    public ElementPosition[] PlayerPos => new ElementPosition[] { PlayerPosLeftEdge, PlayerPosLeftCenter, PlayerPosRightCenter, PlayerPosRightEdge };
    public ElementPosition PlayerPosLeftEdge;
    public ElementPosition PlayerPosLeftCenter;
    public ElementPosition PlayerPosRightCenter;
    public ElementPosition PlayerPosRightEdge;


    [Header("History")]
    public List<AvatarCommand> History => history;
    public List<AvatarCommand> history = new List<AvatarCommand>(16);
    public int GetHistoryLenght => history.Select(x => x.duration).Sum();

    public AvatarState GetBallAtTime(int time)
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
    public AvatarCommand CommandAtTime(int time)
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

public enum EAvatarPos
{
    LeftEdge = 0,
    LeftCenter = 1,
    RightCenter = 2,
    RightEdge = 4,
}
[System.Serializable]
public class AvatarState
{
    public EAvatarPos currentPos = EAvatarPos.LeftCenter;
    public bool isInReversMode = false;
    public int progressState = 0;
}

public class WaitCommand : AvatarCommand
{
    public override AvatarState ApplyCommand(AvatarState avatarBeforeCommand, int time)
    {
        var newBallState = avatarBeforeCommand;
        if (time < startTime)
        {
            return newBallState;
        }
        else
        {
            newBallState.progressState = 0;
        }
        return newBallState;
    }
    public WaitCommand(int startTime)
    {
        this.startTime = startTime;
    }

    public override string CommandInfo() => "Wait";
    public override string CommandLog() => $"Wait at {startTime}";
}
public class FlipSideCommand : AvatarCommand
{
    public override AvatarState ApplyCommand(AvatarState avatarBeforeCommand, int time)
    {
        var newBallState = avatarBeforeCommand;
        if (time < startTime)
        {
            return newBallState;
        }
        else
        {
            newBallState.isInReversMode = !newBallState.isInReversMode;
            newBallState.progressState = 0;
        }
        return newBallState;
    }
    public FlipSideCommand(int startTime)
    {
        this.startTime = startTime;
    }

    public override string CommandInfo() => "Wait";
    public override string CommandLog() => $"Wait at {startTime}";
}
public class MoveCommand : AvatarCommand
{
    public EAvatarPos NewPos;

    public override AvatarState ApplyCommand(AvatarState avatarBeforeCommand, int time)
    {
        var newBallState = avatarBeforeCommand;
        if (time < startTime)
        {
            return newBallState;
        }
        else
        {
            newBallState.currentPos = NewPos;
            newBallState.progressState = 0;
        }

        return newBallState;
    }

    public MoveCommand(int startTime, EAvatarPos newPos)
    {
        this.startTime = startTime;
        this.NewPos = newPos;
    }

    public override string CommandInfo() => $"Move to {Enum.GetName(typeof(EAvatarPos), NewPos)}";
    public override string CommandLog() => $"Movement {Enum.GetName(typeof(EAvatarPos), NewPos)} at {startTime} ";
}
public class ActionCommand : AvatarCommand
{
    public EShootType type;

    public override AvatarState ApplyCommand(AvatarState avatarBeforeCommand, int time)
    {
        var avatarBallState = avatarBeforeCommand;
        if (time < startTime)
        {
            return avatarBallState;
        }
        if (time == startTime)
        {
            avatarBallState.progressState = 0;
        }
        else if (startTime < time && time < EndTime)
        {
            avatarBallState.progressState = time - startTime;
        }
        else if (time >= EndTime)
        {
            avatarBallState.progressState = duration;
        }

        return avatarBallState;
    }
    public ActionCommand(int startTime, EShootType actionType) 
    {
        this.startTime = startTime;
        this.duration = actionType.ShootPrepDuration();
        this.type = actionType;
    }

    public void CheckRegisterHit()
    {
        //TODO
    }


    public override string CommandInfo() => $"Prepare {Enum.GetName(typeof(EShootType), type)} Shoot";
    public override string CommandLog() => $"Prepare shoot {Enum.GetName(typeof(EShootType), type)} at {EndTime} ";
}
