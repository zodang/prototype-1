using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ChainUpgradeSystem))]
public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private ChainUpgradeSystem chainUpgradeSystem;
    [SerializeField] private Button branchUpgradeBtn;
    [SerializeField] private Button branchLuckyBtn;
    [SerializeField] private Button nodeUpgradeBtn;
    [SerializeField] private Button nodeLuckyBtn;

    [Header("Branch UI")]
    [SerializeField] private TMP_Text branchCurrentText;
    [SerializeField] private TMP_Text branchMaxText;
    [SerializeField] private TMP_Text branchPercentText;

    [Header("Node UI")]
    [SerializeField] private TMP_Text nodeCurrentText;
    [SerializeField] private TMP_Text nodeMaxText;
    [SerializeField] private TMP_Text nodePercentText;

    private void Awake()
    {
        if (chainUpgradeSystem == null)
        {
            chainUpgradeSystem = GetComponent<ChainUpgradeSystem>();
        }

        if (chainUpgradeSystem == null)
        {
            chainUpgradeSystem = FindFirstObjectByType<ChainUpgradeSystem>();
        }
    }

    private void OnEnable()
    {
        if (chainUpgradeSystem != null)
        {
            chainUpgradeSystem.OnUpgradeStateChanged += UpdateUI;
        }

        if (branchUpgradeBtn != null)
        {
            branchUpgradeBtn.onClick.AddListener(UpgradeChainBranch);
        }

        if (branchLuckyBtn != null)
        {
            branchLuckyBtn.onClick.AddListener(IncreaseBranchLuckChance);
        }

        if (nodeUpgradeBtn != null)
        {
            nodeUpgradeBtn.onClick.AddListener(UpgradeChainNode);
        }

        if (nodeLuckyBtn != null)
        {
            nodeLuckyBtn.onClick.AddListener(IncreaseNodeLuckChance);
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (chainUpgradeSystem != null)
        {
            chainUpgradeSystem.OnUpgradeStateChanged -= UpdateUI;
        }

        if (branchUpgradeBtn != null)
        {
            branchUpgradeBtn.onClick.RemoveListener(UpgradeChainBranch);
        }

        if (branchLuckyBtn != null)
        {
            branchLuckyBtn.onClick.RemoveListener(IncreaseBranchLuckChance);
        }

        if (nodeUpgradeBtn != null)
        {
            nodeUpgradeBtn.onClick.RemoveListener(UpgradeChainNode);
        }

        if (nodeLuckyBtn != null)
        {
            nodeLuckyBtn.onClick.RemoveListener(IncreaseNodeLuckChance);
        }
    }

    private void UpgradeChainBranch()
    {
        if (chainUpgradeSystem == null) return;

        chainUpgradeSystem.TryUpgradeBranch();
        UpdateUI();
    }

    private void UpgradeChainNode()
    {
        if (chainUpgradeSystem == null) return;

        chainUpgradeSystem.TryUpgradeNode();
        UpdateUI();
    }

    private void IncreaseBranchLuckChance()
    {
        if (chainUpgradeSystem == null) return;

        chainUpgradeSystem.IncreaseBranchLuckChance();
        UpdateUI();
    }

    private void IncreaseNodeLuckChance()
    {
        if (chainUpgradeSystem == null) return;

        chainUpgradeSystem.IncreaseNodeLuckChance();
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateButtons();
        UpdateTexts();
    }

    private void UpdateButtons()
    {
        if (branchUpgradeBtn != null)
        {
            branchUpgradeBtn.interactable = chainUpgradeSystem != null && chainUpgradeSystem.CanUpgradeBranch;
        }

        if (branchLuckyBtn != null)
        {
            branchLuckyBtn.interactable = chainUpgradeSystem != null && chainUpgradeSystem.CanIncreaseBranchLuck;
        }

        if (nodeUpgradeBtn != null)
        {
            nodeUpgradeBtn.interactable = chainUpgradeSystem != null && chainUpgradeSystem.CanUpgradeNode;
        }

        if (nodeLuckyBtn != null)
        {
            nodeLuckyBtn.interactable = chainUpgradeSystem != null && chainUpgradeSystem.CanIncreaseNodeLuck;
        }
    }

    private void UpdateTexts()
    {
        if (chainUpgradeSystem == null) return;

        UpdateUpgradeTexts(
            branchCurrentText,
            branchMaxText,
            branchPercentText,
            chainUpgradeSystem.BranchCurrentLevel,
            chainUpgradeSystem.BranchMaxLevel,
            chainUpgradeSystem.BranchCurrentSuccessChance);

        UpdateUpgradeTexts(
            nodeCurrentText,
            nodeMaxText,
            nodePercentText,
            chainUpgradeSystem.NodeCurrentLevel,
            chainUpgradeSystem.NodeMaxLevel,
            chainUpgradeSystem.NodeCurrentSuccessChance);
    }

    private void UpdateUpgradeTexts(TMP_Text currentText, TMP_Text maxText, TMP_Text percentText, int currentLevel, int maxLevel, float successChance)
    {
        if (currentText != null)
        {
            currentText.text = currentLevel.ToString();
        }

        if (maxText != null)
        {
            maxText.text = maxLevel.ToString();
        }

        if (percentText != null)
        {
            percentText.text = successChance.ToString("P0");
        }
    }
}
