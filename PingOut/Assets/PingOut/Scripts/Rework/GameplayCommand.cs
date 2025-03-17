[System.Serializable]
public abstract class GameplayCommand
{
    public int startTime { get; set; } = 0;
    public int duration { get; set; } = 1;
    public int EndTime => startTime + duration;
    public abstract string CommandInfo();
    public abstract string CommandLog();
}

[System.Serializable]
public abstract class BallCommand : GameplayCommand
{
    public abstract BallState ApplyCommand(BallState ballBeforeCommand, int time);
}
[System.Serializable]
public abstract class AvatarCommand : GameplayCommand
{
    public abstract AvatarState ApplyCommand(AvatarState avatarBeforeCommand, int time);
}