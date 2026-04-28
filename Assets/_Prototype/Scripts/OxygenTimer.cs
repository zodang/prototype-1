using System;
using UnityEngine;
using UnityEngine.Events;

public class OxygenTimer : MonoBehaviour
{
    [SerializeField] private float duration = 30f;
    [SerializeField] private bool startOnPlay = true;
    public event Action onOxygenEnded;

    private const float TickInterval = 1f;

    private float remainingTime;
    private float tickTimer;
    private bool hasEnded;

    public event Action OnOxygenEnded;
    public event Action<float, float> OnOxygenChanged;

    public float Duration => duration;
    public float RemainingTime => remainingTime;
    public float NormalizedRemainingTime => duration <= 0f ? 0f : Mathf.Clamp01(remainingTime / duration);
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        remainingTime = Mathf.Max(0f, duration);
    }

    private void Start()
    {
        if (startOnPlay)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        tickTimer += Time.deltaTime;

        while (tickTimer >= TickInterval && IsRunning)
        {
            tickTimer -= TickInterval;
            DecreaseOxygen();
        }
    }

    public void StartTimer()
    {
        remainingTime = Mathf.Max(0f, duration);
        tickTimer = 0f;
        hasEnded = false;
        IsRunning = true;
        OnOxygenChanged?.Invoke(remainingTime, duration);

        if (remainingTime <= 0f)
        {
            EndTimer();
        }
    }

    public void StopTimer()
    {
        IsRunning = false;
        tickTimer = 0f;
    }

    public void SetDuration(float seconds)
    {
        duration = Mathf.Max(0f, seconds);
        remainingTime = Mathf.Min(remainingTime, duration);
        tickTimer = 0f;
        OnOxygenChanged?.Invoke(remainingTime, duration);
    }

    private void DecreaseOxygen()
    {
        remainingTime = Mathf.Max(0f, remainingTime - 1f);
        OnOxygenChanged?.Invoke(remainingTime, duration);

        if (remainingTime <= 0f)
        {
            EndTimer();
        }
    }

    private void EndTimer()
    {
        if (hasEnded)
        {
            return;
        }

        remainingTime = 0f;
        hasEnded = true;
        IsRunning = false;

        OnOxygenEnded?.Invoke();
        onOxygenEnded?.Invoke();
    }
}
