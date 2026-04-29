using System;
using System.Collections.Generic;
using UnityEngine;

public class ChainUpgradeSystem : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private GemManager gemManager;
    [SerializeField] private ChainUpgradeData branchUpgrade = new ChainUpgradeData("Branch");
    [SerializeField] private ChainUpgradeData nodeUpgrade = new ChainUpgradeData("Node");
    [SerializeField, Range(0f, 1f)] private float luckChanceIncreaseAmount = 0.1f;

    public bool CanUpgradeBranch => branchUpgrade.CanUpgrade && CanUseGems(branchUpgrade.UpgradeGemCost);
    public bool CanUpgradeNode => nodeUpgrade.CanUpgrade && CanUseGems(nodeUpgrade.UpgradeGemCost);
    public bool CanIncreaseBranchLuck => branchUpgrade.CanIncreaseCurrentSuccessChance && CanUseGems(branchUpgrade.LuckGemCost);
    public bool CanIncreaseNodeLuck => nodeUpgrade.CanIncreaseCurrentSuccessChance && CanUseGems(nodeUpgrade.LuckGemCost);
    public int BranchCurrentLevel => branchUpgrade.CurrentLevel;
    public int BranchMaxLevel => branchUpgrade.MaxLevel;
    public float BranchCurrentSuccessChance => branchUpgrade.CurrentSuccessChance;
    public int BranchUpgradeGemCost => branchUpgrade.UpgradeGemCost;
    public int BranchLuckGemCost => branchUpgrade.LuckGemCost;
    public int NodeCurrentLevel => nodeUpgrade.CurrentLevel;
    public int NodeMaxLevel => nodeUpgrade.MaxLevel;
    public float NodeCurrentSuccessChance => nodeUpgrade.CurrentSuccessChance;
    public int NodeUpgradeGemCost => nodeUpgrade.UpgradeGemCost;
    public int NodeLuckGemCost => nodeUpgrade.LuckGemCost;
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

        if (gemManager == null)
        {
            gemManager = FindFirstObjectByType<GemManager>();
        }
    }

    private void OnEnable()
    {
        if (gemManager != null)
        {
            gemManager.OnGemCountChanged += HandleGemCountChanged;
        }
    }

    private void OnDisable()
    {
        if (gemManager != null)
        {
            gemManager.OnGemCountChanged -= HandleGemCountChanged;
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
        if (!TryUseGems(upgradeData.UpgradeGemCost)) return false;

        bool upgraded = upgradeData.TryUpgrade();
        if (upgraded)
        {
            applyUpgrade?.Invoke();
        }

        OnUpgradeStateChanged?.Invoke();
        return upgraded;
    }

    private bool IncreaseLuckChance(ChainUpgradeData upgradeData)
    {
        if (!upgradeData.CanIncreaseCurrentSuccessChance) return false;
        if (!TryUseGems(upgradeData.LuckGemCost)) return false;

        bool increased = upgradeData.IncreaseCurrentSuccessChance(luckChanceIncreaseAmount);
        OnUpgradeStateChanged?.Invoke();
        return increased;
    }

    private bool CanUseGems(int amount)
    {
        return gemManager != null && gemManager.CanUseGems(amount);
    }

    private bool TryUseGems(int amount)
    {
        if (gemManager == null) return false;

        bool used = gemManager.TryUseGems(amount);
        if (!used)
        {
            Debug.Log($"Not enough gems. Required: {amount}, Current: {gemManager.GemCount}");
        }

        return used;
    }

    private void HandleGemCountChanged(int gemCount)
    {
        OnUpgradeStateChanged?.Invoke();
    }
}

[Serializable]
public class ChainUpgradeData
{
    [SerializeField] private string upgradeName;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int upgradeGemCost = 1;
    [SerializeField] private int luckGemCost = 1;
    [SerializeField, Range(0f, 1f)] private List<float> successChances = new List<float> { 0.8f, 0.6f, 0.4f, 0.2f };

    public bool CanUpgrade => currentLevel < MaxLevel;
    public bool CanIncreaseCurrentSuccessChance => CanUpgrade && successChances[currentLevel - 1] < 1f;
    public int CurrentLevel => currentLevel;
    public int MaxLevel => successChances.Count + 1;
    public float CurrentSuccessChance => CanUpgrade ? successChances[currentLevel - 1] : 1f;
    public int UpgradeGemCost => upgradeGemCost;
    public int LuckGemCost => luckGemCost;

    public ChainUpgradeData(string upgradeName)
    {
        this.upgradeName = upgradeName;
    }

    public void Validate()
    {
        currentLevel = Mathf.Max(1, currentLevel);
        upgradeGemCost = Mathf.Max(0, upgradeGemCost);
        luckGemCost = Mathf.Max(0, luckGemCost);

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
}
