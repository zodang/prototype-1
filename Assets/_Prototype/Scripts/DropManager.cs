using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField] private DroppedCoin droppedCoinPrefab;

    public int minDropCount = 3;
    public int maxDropCount = 6;

    public void Drop(Vector3 position)
    {
        int count = Random.Range(minDropCount, maxDropCount + 1);

        for (int i = 0; i < count; i++)
        {
            DroppedCoin coin = Instantiate(droppedCoinPrefab, position, Quaternion.identity);
            coin.Init();
        }
    }
}
