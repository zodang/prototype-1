using System;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    [SerializeField] private int initialGemCount = 0;

    private int gemCount;

    public int GemCount => gemCount;
    public event Action<int> OnGemCountChanged;

    private void Awake()
    {
        SetGemCount(initialGemCount);
    }

    private void OnEnable()
    {
        DroppedGem.OnCollected += HandleGemCollected;
    }

    private void OnDisable()
    {
        DroppedGem.OnCollected -= HandleGemCollected;
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;

        SetGemCount(gemCount + amount);
    }

    public bool CanUseGems(int amount)
    {
        return amount >= 0 && gemCount >= amount;
    }

    public bool TryUseGems(int amount)
    {
        if (!CanUseGems(amount)) return false;

        SetGemCount(gemCount - amount);
        return true;
    }

    public void UseGems(int amount)
    {
        TryUseGems(amount);
    }

    private void HandleGemCollected(DroppedGem gem)
    {
        AddGems(1);
    }

    private void SetGemCount(int value)
    {
        gemCount = Mathf.Max(0, value);
        OnGemCountChanged?.Invoke(gemCount);
    }
}
