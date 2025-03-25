using NaughtyAttributes;
using UnityEngine;

public class AvatarGhost : MonoBehaviour
{
    public GameTimeManager gameTimeManager;
    public IAvatarHistory avatar;

    [Header("Settings")]
    public bool isPlayer;
    [Range(0, 3)] public int TimeOffset = 0;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        if (isPlayer)
        {
            avatar = PlayerHistory.Instance;
        }
        else
        {
            avatar = AdversaireHistory.Instance;
        }
    }
    public void UpdateToTime()
    {
        var time = gameTimeManager.CurrentTick + TimeOffset;
        var avatarState = avatar.GetAvatarAtTime(time);

        transform.position = avatar.GetPosition(avatarState.currentPos);
        spriteRenderer.flipX = avatarState.isInReversMode;
    }
}
