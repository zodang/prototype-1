using UnityEngine;
using UnityEngine.UI;

public class HpFillUI : MonoBehaviour
{
    [SerializeField] private PlayerHp hpTimer;
    [SerializeField] private Image fillUI;

    private void Awake()
    {
        if (fillUI == null)
        {
            fillUI = GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (hpTimer == null)
        {
            return;
        }

        hpTimer.OnHpChanged += HandleHpChanged;
        UpdateFill();
    }

    private void OnDisable()
    {
        if (hpTimer == null)
        {
            return;
        }

        hpTimer.OnHpChanged -= HandleHpChanged;
    }

    public void SetTimer(PlayerHp timer)
    {
        if (hpTimer != null)
        {
            hpTimer.OnHpChanged -= HandleHpChanged;
        }

        hpTimer = timer;

        if (isActiveAndEnabled && hpTimer != null)
        {
            hpTimer.OnHpChanged += HandleHpChanged;
        }

        UpdateFill();
    }

    private void HandleHpChanged(float currentHp, float maxHp)
    {
        SetFill(maxHp <= 0f ? 0f : currentHp / maxHp);
    }

    private void UpdateFill()
    {
        if (hpTimer == null)
        {
            return;
        }

        SetFill(hpTimer.NormalizedHp);
    }

    private void SetFill(float ratio)
    {
        if (fillUI == null)
        {
            return;
        }

        fillUI.fillAmount = Mathf.Clamp01(ratio);
    }
}
