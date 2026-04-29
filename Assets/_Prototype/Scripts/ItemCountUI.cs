using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemCountUI : MonoBehaviour
{
    [FormerlySerializedAs("gemManager")]
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private TMP_Text countText;

    private void Awake()
    {
        if (coinManager == null)
        {
            coinManager = FindFirstObjectByType<CoinManager>();
        }

        if (countText == null)
        {
            countText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (coinManager == null)
        {
            coinManager = FindFirstObjectByType<CoinManager>();
        }

        if (coinManager == null) return;

        coinManager.OnCoinCountChanged += UpdateCount;
        UpdateCount(coinManager.CoinCount);
    }

    private void OnDisable()
    {
        if (coinManager == null) return;

        coinManager.OnCoinCountChanged -= UpdateCount;
    }

    private void UpdateCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }
}
