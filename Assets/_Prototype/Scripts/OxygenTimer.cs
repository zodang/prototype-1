using System;
using UnityEngine;

public class OxygenTimer : MonoBehaviour
{
    [SerializeField] private float duration = 30f;
    [SerializeField] private bool startOnPlay = true;

    private float remainingTime;
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

        remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
        OnOxygenChanged?.Invoke(remainingTime, duration);

        if (remainingTime <= 0f)
        {
            EndTimer();
        }
    }

    public void StartTimer()
    {
        remainingTime = Mathf.Max(0f, duration);
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
    }

    public void SetDuration(float seconds)
    {
        duration = Mathf.Max(0f, seconds);
        remainingTime = Mathf.Min(remainingTime, duration);
        OnOxygenChanged?.Invoke(remainingTime, duration);
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
        OnOxygenEnded?.Invoke();
    }
}
