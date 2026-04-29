using TMPro;
using UnityEngine;

public class ItemCountUI : MonoBehaviour
{
    [SerializeField] private GemManager gemManager;
    [SerializeField] private TMP_Text countText;

    private void Awake()
    {
        if (gemManager == null)
        {
            gemManager = FindFirstObjectByType<GemManager>();
        }

        if (countText == null)
        {
            countText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (gemManager == null)
        {
            gemManager = FindFirstObjectByType<GemManager>();
        }

        if (gemManager == null) return;

        gemManager.OnGemCountChanged += UpdateCount;
        UpdateCount(gemManager.GemCount);
    }

    private void OnDisable()
    {
        if (gemManager == null) return;

        gemManager.OnGemCountChanged -= UpdateCount;
    }

    private void UpdateCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }
}
