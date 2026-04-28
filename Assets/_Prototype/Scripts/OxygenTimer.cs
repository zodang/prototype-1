using System;
using UnityEngine;

public class OxygenTimer : MonoBehaviour
{
    [SerializeField] private float duration = 30f;
    [SerializeField] private bool startOnPlay = true;

    private float remainingTime;
    private bool hasEnded;

    public event Action OnOxygenEnded;

    public float Duration => duration;
    public float RemainingTime => remainingTime;
    public float NormalizedRemainingTime => duration <= 0f ? 0f : Mathf.Clamp01(remainingTime / duration);

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
        remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);

        if (remainingTime <= 0f)
        {
            EndTimer();
        }
    }

    public void StartTimer()
    {
        remainingTime = Mathf.Max(0f, duration);
        hasEnded = false;

        if (remainingTime <= 0f)
        {
            EndTimer();
        }
    }

    public void SetDuration(float seconds)
    {
        duration = Mathf.Max(0f, seconds);
        remainingTime = Mathf.Min(remainingTime, duration);
    }

    private void EndTimer()
    {
        if (hasEnded)
        {
            return;
        }

        remainingTime = 0f;
        hasEnded = true;

        OnOxygenEnded?.Invoke();
    }
}
