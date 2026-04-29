using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Button branchUpgradeBtn;
    [SerializeField] private Button nodeUpgradeBtn;

    private void Awake()
    {
        if (playerAttack == null)
        {
            playerAttack = FindFirstObjectByType<PlayerAttack>();
        }
    }

    private void OnEnable()
    {
        if (branchUpgradeBtn != null)
        {
            branchUpgradeBtn.onClick.AddListener(UpgradeChainBranch);
        }

        if (nodeUpgradeBtn != null)
        {
            nodeUpgradeBtn.onClick.AddListener(UpgradeChainNode);
        }
    }

    private void OnDisable()
    {
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
        if (playerAttack == null) return;

        playerAttack.IncreaseMaxChainBranchCount();
    }

    private void UpgradeChainNode()
    {
        if (playerAttack == null) return;

        playerAttack.IncreaseMaxChainCount();
    }
}
