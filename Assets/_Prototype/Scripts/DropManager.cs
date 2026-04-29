using UnityEngine;

public class DropManager : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private DroppedCoin droppedCoinPrefab;

    public int minDropCount = 3;
    public int maxDropCount = 6;

    [Header("Gem")]
    [SerializeField] private DroppedGem droppedGemPrefab;
    [SerializeField] private int minGemDropCount = 1;
    [SerializeField] private int maxGemDropCount = 1;

    private void OnValidate()
    {
        minDropCount = Mathf.Max(0, minDropCount);
        maxDropCount = Mathf.Max(minDropCount, maxDropCount);
        minGemDropCount = Mathf.Max(0, minGemDropCount);
        maxGemDropCount = Mathf.Max(minGemDropCount, maxGemDropCount);
    }

    public void Drop(Vector3 position)
    {
        DropCoins(position);
        DropGems(position);
    }

    private void DropCoins(Vector3 position)
    {
        if (droppedCoinPrefab == null) return;

        int count = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < count; i++)
        {
            DroppedCoin coin = Instantiate(droppedCoinPrefab, position, Quaternion.identity);
            coin.Init();
        }
    }

    private void DropGems(Vector3 position)
    {
        if (droppedGemPrefab == null) return;

        int count = Random.Range(minGemDropCount, maxGemDropCount + 1);

        for (int i = 0; i < count; i++)
        {
            DroppedGem gem = Instantiate(droppedGemPrefab, position, Quaternion.identity);
            gem.Init();
        }
    }

    private bool ShouldDrop(float dropChance)
    {
        return dropChance >= 1f || dropChance > 0f && Random.value < dropChance;
    }
}
