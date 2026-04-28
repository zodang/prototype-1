using UnityEngine;
using UnityEngine.UI;

public class OxygenFillUI : MonoBehaviour
{
    [SerializeField] private OxygenTimer oxygenTimer;
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
        if (oxygenTimer == null)
        {
            return;
        }

        oxygenTimer.OnOxygenChanged += HandleOxygenChanged;
        UpdateFill();
    }

    private void OnDisable()
    {
        if (oxygenTimer == null)
        {
            return;
        }

        oxygenTimer.OnOxygenChanged -= HandleOxygenChanged;
    }

    public void SetTimer(OxygenTimer timer)
    {
        if (oxygenTimer != null)
        {
            oxygenTimer.OnOxygenChanged -= HandleOxygenChanged;
        }

        oxygenTimer = timer;

        if (isActiveAndEnabled && oxygenTimer != null)
        {
            oxygenTimer.OnOxygenChanged += HandleOxygenChanged;
        }

        UpdateFill();
    }

    private void HandleOxygenChanged(float remainingTime, float duration)
    {
        SetFill(duration <= 0f ? 0f : remainingTime / duration);
    }

    private void UpdateFill()
    {
        if (oxygenTimer == null)
        {
            return;
        }

        SetFill(oxygenTimer.NormalizedRemainingTime);
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
