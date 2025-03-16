using UnityEngine;
using UnityEngine.Events;

public class AvantageManager : Singleton<AvantageManager>
{
    public Vector2Int AvantageRange = new Vector2Int(-10, 10);
    public int Avantage = 0;

    public UnityEvent<int> OnAvantageChange;
    public UnityEvent<float> OnAvantageChangeF;

    public void Addavantage(int avantage)
    {
        Avantage += avantage;
        Avantage = Mathf.Clamp(Avantage, AvantageRange.x, AvantageRange.y);
        OnAvantageChange?.Invoke(Avantage);
        OnAvantageChangeF?.Invoke((float)Avantage);
    }
}
