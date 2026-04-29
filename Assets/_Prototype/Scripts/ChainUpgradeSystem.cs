using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChainUpgradeSystem : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    [FormerlySerializedAs("gemManager")]
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private ChainUpgradeData branchUpgrade = new ChainUpgradeData("Branch");
    [SerializeField] private ChainUpgradeData nodeUpgrade = new ChainUpgradeData("Node");
    [SerializeField, Range(0f, 1f)] private float luckChanceIncreaseAmount = 0.1f;

    public bool CanUpgradeBranch => branchUpgrade.CanUpgrade && CanUseCoins(branchUpgrade.UpgradeCoinCost);
    public bool CanUpgradeNode => nodeUpgrade.CanUpgrade && CanUseCoins(nodeUpgrade.UpgradeCoinCost);
    public bool CanIncreaseBranchLuck => branchUpgrade.CanIncreaseCurrentSuccessChance && CanUseCoins(branchUpgrade.LuckCoinCost);
    public bool CanIncreaseNodeLuck => nodeUpgrade.CanIncreaseCurrentSuccessChance && CanUseCoins(nodeUpgrade.LuckCoinCost);
    public int BranchCurrentLevel => branchUpgrade.CurrentLevel;
    public int BranchMaxLevel => branchUpgrade.MaxLevel;
    public float BranchCurrentSuccessChance => branchUpgrade.CurrentSuccessChance;
    public int BranchUpgradeCoinCost => branchUpgrade.UpgradeCoinCost;
    public int BranchLuckCoinCost => branchUpgrade.LuckCoinCost;
    public int NodeCurrentLevel => nodeUpgrade.CurrentLevel;
    public int NodeMaxLevel => nodeUpgrade.MaxLevel;
    public float NodeCurrentSuccessChance => nodeUpgrade.CurrentSuccessChance;
    public int NodeUpgradeCoinCost => nodeUpgrade.UpgradeCoinCost;
    public int NodeLuckCoinCost => nodeUpgrade.LuckCoinCost;
    public event Action OnUpgradeStateChanged;

    private void OnValidate()
    {
        luckChanceIncreaseAmount = Mathf.Clamp01(luckChanceIncreaseAmount);
        branchUpgrade.Validate();
        nodeUpgrade.Validate();
    }

    private void Awake()
    {
        if (playerAttack == null)
        {
            playerAttack = FindFirstObjectByType<PlayerAttack>();
        }

        if (coinManager == null)
        {
            coinManager = FindFirstObjectByType<CoinManager>();
        }
    }

    private void OnEnable()
    {
        if (coinManager != null)
        {
            coinManager.OnCoinCountChanged += HandleCoinCountChanged;
        }
    }

    private void OnDisable()
    {
        if (coinManager != null)
        {
            coinManager.OnCoinCountChanged -= HandleCoinCountChanged;
        }
    }

    public bool TryUpgradeBranch()
    {
        return TryUpgrade(branchUpgrade, () => playerAttack.IncreaseMaxChainBranchCount());
    }

    public bool TryUpgradeNode()
    {
        return TryUpgrade(nodeUpgrade, () => playerAttack.IncreaseMaxChainCount());
    }

    public bool IncreaseBranchLuckChance()
    {
        return IncreaseLuckChance(branchUpgrade);
    }

    public bool IncreaseNodeLuckChance()
    {
        return IncreaseLuckChance(nodeUpgrade);
    }

    private bool TryUpgrade(ChainUpgradeData upgradeData, Action applyUpgrade)
    {
        if (playerAttack == null) return false;
        if (!upgradeData.CanUpgrade) return false;
        if (!TryUseCoins(upgradeData.UpgradeCoinCost)) return false;

        bool upgraded = upgradeData.TryUpgrade();
        if (upgraded)
        {
            applyUpgrade?.Invoke();
        }

        upgradeData.IncreaseUpgradeCoinCost();
        OnUpgradeStateChanged?.Invoke();
        return upgraded;
    }

    private bool IncreaseLuckChance(ChainUpgradeData upgradeData)
    {
        if (!upgradeData.CanIncreaseCurrentSuccessChance) return false;
        if (!TryUseCoins(upgradeData.LuckCoinCost)) return false;

        bool increased = upgradeData.IncreaseCurrentSuccessChance(luckChanceIncreaseAmount);
        OnUpgradeStateChanged?.Invoke();
        return increased;
    }

    private bool CanUseCoins(int amount)
    {
        return coinManager != null && coinManager.CanUseCoins(amount);
    }

    private bool TryUseCoins(int amount)
    {
        if (coinManager == null) return false;

        bool used = coinManager.TryUseCoins(amount);
        if (!used)
        {
            Debug.Log($"Not enough coins. Required: {amount}, Current: {coinManager.CoinCount}");
        }

        return used;
    }

    private void HandleCoinCountChanged(int coinCount)
    {
        OnUpgradeStateChanged?.Invoke();
    }
}

[Serializable]
public class ChainUpgradeData
{
    [SerializeField] private string upgradeName;
    [SerializeField] private int currentLevel = 1;
    [FormerlySerializedAs("upgradeGemCost")]
    [SerializeField] private int upgradeCoinCost = 1;
    [FormerlySerializedAs("upgradeGemCostIncrease")]
    [SerializeField] private int upgradeCoinCostIncrease = 1;
    [FormerlySerializedAs("luckGemCost")]
    [SerializeField] private int luckCoinCost = 1;
    [SerializeField, Range(0f, 1f)] private List<float> successChances = new List<float> { 0.8f, 0.6f, 0.4f, 0.2f };

    public bool CanUpgrade => currentLevel < MaxLevel;
    public bool CanIncreaseCurrentSuccessChance => CanUpgrade && successChances[currentLevel - 1] < 1f;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => successChances.Count + 1;
    public float CurrentSuccessChance => CanUpgrade ? successChances[currentLevel - 1] : 1f;
    public int UpgradeCoinCost => upgradeCoinCost;
    public int LuckCoinCost => luckCoinCost;

    public ChainUpgradeData(string upgradeName)
    {
        this.upgradeName = upgradeName;
    }

    public void Validate()
    {
        currentLevel = Mathf.Max(1, currentLevel);
        upgradeCoinCost = Mathf.Max(0, upgradeCoinCost);
        upgradeCoinCostIncrease = Mathf.Max(0, upgradeCoinCostIncrease);
        luckCoinCost = Mathf.Max(0, luckCoinCost);

        for (int i = 0; i < successChances.Count; i++)
        {
            successChances[i] = Mathf.Clamp01(successChances[i]);
        }

        currentLevel = Mathf.Min(currentLevel, MaxLevel);
    }

    public bool TryUpgrade()
    {
        Validate();

        if (!CanUpgrade)
        {
            Debug.Log($"{upgradeName} is max level.");
            return false;
        }

        int chanceIndex = currentLevel - 1;
        float successChance = successChances[chanceIndex];
        float roll = UnityEngine.Random.value;

        if (roll > successChance)
        {
            Debug.Log($"FAILED: {currentLevel}/{MaxLevel} ({successChance:P0})");
            return false;
        }

        int previousLevel = currentLevel;
        currentLevel++;
        Debug.Log($"SUCCESS: {currentLevel}/{MaxLevel} ({successChance:P0})");
        return true;
        
        // Debug.Log($"{upgradeName} upgrade succeeded. Chance: {successChance:P0}, Roll: {roll:P0}, Level: {previousLevel} -> {currentLevel}/{MaxLevel}");
    }

    public bool IncreaseCurrentSuccessChance(float amount)
    {
        Validate();

        if (!CanUpgrade)
        {
            Debug.Log($"{upgradeName} is max level.");
            return false;
        }

        int chanceIndex = currentLevel - 1;
        float previousChance = successChances[chanceIndex];
        float nextChance = Mathf.Clamp01(previousChance + amount);

        if (Mathf.Approximately(previousChance, nextChance))
        {
            Debug.Log($"{upgradeName} chance is already max. Chance: {previousChance:P0}");
            return false;
        }

        successChances[chanceIndex] = nextChance;
        Debug.Log($"{upgradeName} chance increased. Chance: {previousChance:P0} -> {nextChance:P0}");
        return true;
    }

    public void IncreaseUpgradeCoinCost()
    {
        if (upgradeCoinCostIncrease <= 0) return;

        upgradeCoinCost += upgradeCoinCostIncrease;
        Debug.Log($"{upgradeName} upgrade cost increased. Cost: {upgradeCoinCost}");
    }
}
