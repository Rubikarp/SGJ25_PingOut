using UnityEngine;

public class BallController : MonoBehaviour
{
    public PlayerPosition shootPos;
    public int shootTick = 0;

    public PlayerPosition aimPos;
    public int tickToDestination;

    private void Awake()
    {
        GameTimeManager.OnTickChange += TickMove;
    }

    public  void TickMove(int currentTick, int targetTick)
    {

    }
}
