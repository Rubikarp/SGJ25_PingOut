using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IAvatarHistory
{
    public AvatarState InitialState { get; }
    public List<AvatarCommand> History { get; }
    public AvatarState GetAvatarAtTime(int time);
    public AvatarCommand GetCommandAtTime(int time);

    public UnityAction<List<AvatarCommand>> OnHistoryChange { get; set; }

    public Vector3 GetPosition(EAvatarPos pos);
}
