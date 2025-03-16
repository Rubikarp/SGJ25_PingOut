using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GameTimeManager : Singleton<GameTimeManager>
{
    public const float TICK_DURATION = .2f;

    public int CurrentTick
    {
        get => _currentTick;
        private set
        {
            if (value < 0)
            {
                Debug.LogWarning("tick cannot be negative", this);
                value = 0;
            }

            if (value == _currentTick) return;
            OnTickChange?.Invoke(_currentTick, value);
            OnTickRefresh?.Invoke(value);
            _currentTick = value;
        }
    }
    [SerializeField, ReadOnly] private int _currentTick = 0;

    [field: SerializeField, ReadOnly] public int SequenceStart { get; private set; } = 0;
    [SerializeField, ReadOnly] private int sequenceDuration = 4;
    public int RestingTick => (SequenceStart + sequenceDuration) - CurrentTick;

    public static UnityAction<int> OnSequenceStart;
    public static UnityAction<int, int> OnTickChange;

    public UnityEvent<float> OnTickRefresh;

    [Button] public void NextTick() => CurrentTick++;
    [Button] public void PrevTick() => CurrentTick--;

    public void SetTick(float tick) => CurrentTick = Mathf.RoundToInt(tick);
    public void AddTick(int tick)
    {
        if (CurrentTick == tick) return;

        CurrentTick += tick;
    }
}
