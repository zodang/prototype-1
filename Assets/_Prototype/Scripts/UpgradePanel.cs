using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ChainUpgradeSystem))]
public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private ChainUpgradeSystem chainUpgradeSystem;
    [SerializeField] private Button branchUpgradeBtn;
    [SerializeField] private Button nodeUpgradeBtn;

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
            chainUpgradeSystem.OnUpgradeStateChanged += UpdateButtons;
        }

        if (branchUpgradeBtn != null)
        {
            branchUpgradeBtn.onClick.AddListener(UpgradeChainBranch);
        }

        if (nodeUpgradeBtn != null)
        {
            nodeUpgradeBtn.onClick.AddListener(UpgradeChainNode);
        }

        UpdateButtons();
    }

    private void OnDisable()
    {
        if (chainUpgradeSystem != null)
        {
            chainUpgradeSystem.OnUpgradeStateChanged -= UpdateButtons;
        }

        if (branchUpgradeBtn != null)
        {
            branchUpgradeBtn.onClick.RemoveListener(UpgradeChainBranch);
        }

        if (nodeUpgradeBtn != null)
        {
            nodeUpgradeBtn.onClick.RemoveListener(UpgradeChainNode);
        }
    }

    private void UpgradeChainBranch()
    {
        if (chainUpgradeSystem == null) return;

        chainUpgradeSystem.TryUpgradeBranch();
        UpdateButtons();
    }

    private void UpgradeChainNode()
    {
        if (chainUpgradeSystem == null) return;

        chainUpgradeSystem.TryUpgradeNode();
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        if (branchUpgradeBtn != null)
        {
            branchUpgradeBtn.interactable = chainUpgradeSystem != null && chainUpgradeSystem.CanUpgradeBranch;
        }

        if (nodeUpgradeBtn != null)
        {
            nodeUpgradeBtn.interactable = chainUpgradeSystem != null && chainUpgradeSystem.CanUpgradeNode;
        }
    }
}
