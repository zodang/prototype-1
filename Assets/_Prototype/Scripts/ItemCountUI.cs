using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemCountUI : MonoBehaviour
{
    private enum ItemType
    {
        Coin,
        Gem,
        CurrentRunGem,
        TotalGem
    }

    [SerializeField] private ItemType itemType = ItemType.Coin;
    [FormerlySerializedAs("gemManager")]
    [SerializeField] private CoinManager coinManager;
    [SerializeField] private GemManager gemManager;
    [SerializeField] private TMP_Text countText;

    private void Awake()
    {
        FindMissingManager();
        
        if (countText == null)
        {
            countText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        FindMissingManager();

        if (itemType == ItemType.Coin)
        {
            if (coinManager == null) return;

            coinManager.OnCoinCountChanged += UpdateCount;
            UpdateCount(coinManager.CoinCount);
            return;
        }

        if (itemType == ItemType.CurrentRunGem)
        {
            if (SceneDataStore.Instance == null) return;

            SceneDataStore.Instance.OnCurrentRunGemCountChanged += UpdateCount;
            UpdateCount(SceneDataStore.Instance.CurrentRunGemCount);
            return;
        }

        if (itemType == ItemType.TotalGem)
        {
            if (SceneDataStore.Instance == null) return;

            SceneDataStore.Instance.OnTotalGemCountChanged += UpdateCount;
            UpdateCount(SceneDataStore.Instance.TotalGemCount);
            return;
        }

        if (gemManager == null) return;

        gemManager.OnGemCountChanged += UpdateCount;
        UpdateCount(gemManager.GemCount);
    }

    private void OnDisable()
    {
        if (coinManager != null)
        {
            coinManager.OnCoinCountChanged -= UpdateCount;
        }

        if (gemManager != null)
        {
            gemManager.OnGemCountChanged -= UpdateCount;
        }

        if (SceneDataStore.Instance != null)
        {
            SceneDataStore.Instance.OnCurrentRunGemCountChanged -= UpdateCount;
            SceneDataStore.Instance.OnTotalGemCountChanged -= UpdateCount;
        }
    }

    private void UpdateCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }

    private void FindMissingManager()
    {
        if (itemType == ItemType.Coin && coinManager == null)
        {
            coinManager = FindFirstObjectByType<CoinManager>();
        }
        else if (itemType == ItemType.Gem && gemManager == null)
        {
            gemManager = FindFirstObjectByType<GemManager>();
        }
    }
}
