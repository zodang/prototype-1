using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChainAttackUI : MonoBehaviour
{
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Image chainFillImg;
    [SerializeField] private TMP_Text chainCount;
    [SerializeField] private TMP_Text chainMaxCount;

    private void OnEnable()
    {
        if (playerAttack == null)
        {
            playerAttack = FindFirstObjectByType<PlayerAttack>();
        }

        if (playerAttack == null)
        {
            return;
        }

        playerAttack.OnChainAttackAmountChanged += HandleChainAttackAmountChanged;
        UpdateUI(playerAttack.CurrentChainAttackAmount, playerAttack.MaxChainAttackAmount);
    }

    private void OnDisable()
    {
        if (playerAttack == null)
        {
            return;
        }

        playerAttack.OnChainAttackAmountChanged -= HandleChainAttackAmountChanged;
    }

    private void HandleChainAttackAmountChanged(float currentAmount, float maxAmount)
    {
        UpdateUI(currentAmount, maxAmount);
    }

    private void UpdateUI(float currentAmount, float maxAmount)
    {
        if (chainFillImg != null)
        {
            chainFillImg.fillAmount = maxAmount <= 0f ? 0f : Mathf.Clamp01(currentAmount / maxAmount);
        }

        if (chainCount != null)
        {
            chainCount.text = Mathf.CeilToInt(currentAmount).ToString();
        }

        if (chainMaxCount != null)
        {
            chainMaxCount.text = Mathf.CeilToInt(maxAmount).ToString();
        }
    }
}
