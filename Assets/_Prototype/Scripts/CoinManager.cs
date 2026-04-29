using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CoinManager : MonoBehaviour
{
    [FormerlySerializedAs("initialGemCount")]
    [SerializeField] private int initialCoinCount = 0;

    private int coinCount;

    public int CoinCount => coinCount;
    public event Action<int> OnCoinCountChanged;

    private void Awake()
    {
        SetCoinCount(initialCoinCount);
    }

    private void OnEnable()
    {
        DroppedCoin.OnCollected += HandleCoinCollected;
    }

    private void OnDisable()
    {
        DroppedCoin.OnCollected -= HandleCoinCollected;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        SetCoinCount(coinCount + amount);
    }

    public bool CanUseCoins(int amount)
    {
        return amount >= 0 && coinCount >= amount;
    }

    public bool TryUseCoins(int amount)
    {
        if (!CanUseCoins(amount)) return false;

        SetCoinCount(coinCount - amount);
        return true;
    }

    public void UseCoins(int amount)
    {
        TryUseCoins(amount);
    }

    private void HandleCoinCollected(DroppedCoin coin)
    {
        AddCoins(1);
    }

    private void SetCoinCount(int value)
    {
        coinCount = Mathf.Max(0, value);
        OnCoinCountChanged?.Invoke(coinCount);
    }
}
