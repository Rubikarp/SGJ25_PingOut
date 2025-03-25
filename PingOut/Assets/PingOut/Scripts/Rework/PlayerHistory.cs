using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class PlayerHistory : Singleton<PlayerHistory>, IAvatarHistory
{
    public UnityAction<List<AvatarCommand>> OnHistoryChange { get => onHistoryChange; set => onHistoryChange = value; }
    public UnityAction<List<AvatarCommand>> onHistoryChange;

    [Header("Settings"), SerializeField]
    private AvatarState initialState = new AvatarState();
    public AvatarState InitialState => initialState;

    [Header("Position Data")]
    public ElementPosition PlayerPosLeftEdge;
    public ElementPosition PlayerPosLeftCenter;
    public ElementPosition PlayerPosRightCenter;
    public ElementPosition PlayerPosRightEdge;
    public ElementPosition[] PlayerPos => new ElementPosition[] { PlayerPosLeftEdge, PlayerPosLeftCenter, PlayerPosRightCenter, PlayerPosRightEdge };
    public Vector3 GetPosition(EAvatarPos pos) => PlayerPos[(int)pos].transform.position;

    [Header("History")]
    public List<AvatarCommand> History => history;
    public List<AvatarCommand> history = new List<AvatarCommand>(16);
    public int GetHistoryLenght => history.Select(x => x.duration).Sum();

    public AvatarState GetAvatarAtTime(int time)
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
    public AvatarCommand GetCommandAtTime(int time)
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

    public void AddCommand(AvatarCommand command)
    {
        history.Add(command);
        onHistoryChange?.Invoke(history);
    }
    public void RemoveCommand(AvatarCommand command)
    {
        //Remove all commands that have a superior start time
        history.RemoveAll(x => x.startTime >= command.startTime);
        onHistoryChange?.Invoke(history);
    }

    [Button] public void ResgisterWaitCammand() => AddCommand(new WaitCommand(GetHistoryLenght));
    [Button] public void ResgisterFlipSideCammand() => AddCommand(new FlipSideCommand(GetHistoryLenght));

    public void RegisterMoveCommand(EAvatarPos pos) => AddCommand(new MoveCommand(GetHistoryLenght, pos));
    public void TryMoveToPos(ElementPosition pos)
    {
        EAvatarPos wantedPos = (EAvatarPos)PlayerPos.ToList().IndexOf(pos);
        EAvatarPos currentPos = GetAvatarAtTime(GetHistoryLenght).currentPos;

        if (currentPos == wantedPos)
        {
            Debug.LogWarning("Already at this pos");
        }
        else if (currentPos < wantedPos)
        {
            TryMoveLeft();
        }
        else
        {
            TryMoveRight();
        }
    }
    [Button] public void TryMoveLeft()
    {
        var state = GetAvatarAtTime(GetHistoryLenght);
        if ((int)state.currentPos <= 0)
        {
            Debug.LogWarning("Can't move to the left");
        }
        else
        {
            RegisterMoveCommand(state.currentPos--);
        }
    }
    [Button] public void TryMoveRight()
    {
        var state = GetAvatarAtTime(GetHistoryLenght);
        if ((int)state.currentPos >= Enum.GetValues(typeof(EAvatarPos)).Length)
        {
            Debug.LogWarning("Can't move to the right");
        }
        else
        {
            RegisterMoveCommand(state.currentPos++);
        }
    }

    public void RehisterShootCommand(EShootType shootType) => AddCommand(new ActionCommand(GetHistoryLenght, shootType));
    [Button] public void PrepareShootTopSpin() => RehisterShootCommand(EShootType.TopSpin);
    [Button] public void PrepareShootCoupe() => RehisterShootCommand(EShootType.Coupe);
    [Button] public void PrepareShootBlock() => RehisterShootCommand(EShootType.Block);
    [Button] public void PrepareShootSmash() => RehisterShootCommand(EShootType.Smash);
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

    public override string CommandInfo() => "Flip";
    public override string CommandLog() => $"Flip Side at {startTime}";
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

    public override string CommandInfo() => $"Move";
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

    public override string CommandInfo() => $"{Enum.GetName(typeof(EShootType), type)}";
    public override string CommandLog() => $"Prepare shoot {Enum.GetName(typeof(EShootType), type)} at {EndTime} ";
}