using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private ChainUpgradeSystem chainUpgradeSystem;
    [SerializeField] private RectTransform upgradePanel;
    [SerializeField] private Button foldBtn;
    [SerializeField] private Button branchUpgradeBtn;
    [SerializeField] private Button branchLuckyBtn;
    [SerializeField] private Button nodeUpgradeBtn;
    [SerializeField] private Button nodeLuckyBtn;

    [Header("Fold UI")]
    [SerializeField] private float unfoldedY = 0f;
    [SerializeField] private float foldedY = -300f;
    [SerializeField] private float foldDuration = 0.25f;
    [SerializeField] private Ease foldEase = Ease.OutCubic;

    [Header("Branch UI")]
    [SerializeField] private TMP_Text branchCurrentText;
    [SerializeField] private TMP_Text branchMaxText;
    [SerializeField] private TMP_Text branchPercentText;
    [SerializeField] private TMP_Text branchUpgradeGemText;
    [SerializeField] private TMP_Text branchLuckyGemText;

    [Header("Node UI")]
    [SerializeField] private TMP_Text nodeCurrentText;
    [SerializeField] private TMP_Text nodeMaxText;
    [SerializeField] private TMP_Text nodePercentText;
    [SerializeField] private TMP_Text nodeUpgradeGemText;
    [SerializeField] private TMP_Text nodeLuckyGemText;

    private bool isUnfolded;
    private Tween foldTween;

    private void OnValidate()
    {
        foldDuration = Mathf.Max(0f, foldDuration);
    }

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

        if (upgradePanel == null)
        {
            upgradePanel = GetComponent<RectTransform>();
        }
    }

    private void OnEnable()
    {
        if (chainUpgradeSystem != null)
        {
            chainUpgradeSystem.OnUpgradeStateChanged += UpdateUI;
        }

        if (foldBtn != null)
        {
            foldBtn.onClick.AddListener(ToggleFold);
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

        if (upgradePanel != null)
        {
            isUnfolded = Mathf.Approximately(upgradePanel.anchoredPosition.y, unfoldedY);
        }

        UpdateUI();
    }

    private void OnDisable()
    {
        if (chainUpgradeSystem != null)
        {
            chainUpgradeSystem.OnUpgradeStateChanged -= UpdateUI;
        }

        if (foldBtn != null)
        {
            foldBtn.onClick.RemoveListener(ToggleFold);
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

        foldTween?.Kill();
        foldTween = null;
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

    private void ToggleFold()
    {
        if (upgradePanel == null) return;

        isUnfolded = !isUnfolded;
        float targetY = isUnfolded ? unfoldedY : foldedY;

        foldTween?.Kill();
        foldTween = upgradePanel
            .DOAnchorPosY(targetY, foldDuration)
            .SetEase(foldEase);
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

        UpdateGemCostText(branchUpgradeGemText, chainUpgradeSystem.BranchUpgradeGemCost);
        UpdateGemCostText(branchLuckyGemText, chainUpgradeSystem.BranchLuckGemCost);

        UpdateUpgradeTexts(
            nodeCurrentText,
            nodeMaxText,
            nodePercentText,
            chainUpgradeSystem.NodeCurrentLevel,
            chainUpgradeSystem.NodeMaxLevel,
            chainUpgradeSystem.NodeCurrentSuccessChance);

        UpdateGemCostText(nodeUpgradeGemText, chainUpgradeSystem.NodeUpgradeGemCost);
        UpdateGemCostText(nodeLuckyGemText, chainUpgradeSystem.NodeLuckGemCost);
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

    private void UpdateGemCostText(TMP_Text gemText, int gemCost)
    {
        if (gemText != null)
        {
            gemText.text = gemCost.ToString();
        }
    }
}
