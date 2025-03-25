using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class AdversaireHistory : Singleton<AdversaireHistory>, IAvatarHistory
{
    public UnityAction<List<AvatarCommand>> OnHistoryChange { get => onHistoryChange; set => onHistoryChange = value; }
    public UnityAction<List<AvatarCommand>> onHistoryChange;

    [Header("Settings"), SerializeField]
    private AvatarState initialState = new AvatarState();
    public AvatarState InitialState => initialState;

    [Header("Position Data")]
    public ElementPosition[] EnnemyPos => new ElementPosition[] { EnnemyPosLeftEdge, EnnemyPosLeftCenter, EnnemyPosRightCenter, EnnemyPosRightEdge };
    public ElementPosition EnnemyPosLeftEdge;
    public ElementPosition EnnemyPosLeftCenter;
    public ElementPosition EnnemyPosRightCenter;
    public ElementPosition EnnemyPosRightEdge;

    [Header("History")]
    public List<AvatarCommand> History => history;
    public List<AvatarCommand> history = new List<AvatarCommand>(16);
    public int GetHistoryLenght => history.Select(x => x.duration).Sum();

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

    public Vector3 GetPosition(EAvatarPos pos) => EnnemyPos[(int)pos].transform.position;

    [Button] public void ResgisterWaitCammand() => AddCommand(new WaitCommand(GetHistoryLenght));
    [Button] public void ResgisterFlipSideCammand() => AddCommand(new FlipSideCommand(GetHistoryLenght));

    public void RegisterMoveCommand(EAvatarPos pos) => AddCommand(new MoveCommand(GetHistoryLenght, pos));
    public void TryMoveToPos(ElementPosition pos)
    {
        EAvatarPos wantedPos = (EAvatarPos)EnnemyPos.ToList().IndexOf(pos);
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
    [Button]
    public void TryMoveLeft()
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
    [Button]
    public void TryMoveRight()
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
