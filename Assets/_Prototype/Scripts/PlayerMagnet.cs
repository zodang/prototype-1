using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    [SerializeField] private float magnetRange = 1f;
    [SerializeField] private float magnetSpeed = 15f;
    [SerializeField] private float collectRange = 0.1f;
    [SerializeField] private int maxDetectedItems = 32;

    private Collider2D[] detectedItems;
    private ContactFilter2D itemFilter;

    private void Awake()
    {
        detectedItems = new Collider2D[Mathf.Max(1, maxDetectedItems)];
        itemFilter = ContactFilter2D.noFilter;
    }

    private void Update()
    {
        int count = Physics2D.OverlapCircle(transform.position, magnetRange, itemFilter, detectedItems);

        for (int i = 0; i < count; i++)
        {
            Collider2D itemCollider = detectedItems[i];
            if (itemCollider == null) continue;

            DroppedCoin coin = itemCollider.GetComponent<DroppedCoin>();
            if (coin != null)
            {
                MoveOrCollectCoin(coin);
                continue;
            }

            DroppedGem gem = itemCollider.GetComponent<DroppedGem>();
            if (gem != null)
            {
                MoveOrCollectGem(gem);
            }
        }
    }

    private void MoveOrCollectCoin(DroppedCoin coin)
    {
        if (!coin.CanPickup) return;

        float distance = Vector2.Distance(transform.position, coin.transform.position);
        if (distance <= collectRange)
        {
            coin.Collect();
            return;
        }

        coin.MoveToward(transform.position, magnetSpeed);
    }

    private void MoveOrCollectGem(DroppedGem gem)
    {
        if (!gem.CanPickup) return;

        float distance = Vector2.Distance(transform.position, gem.transform.position);
        if (distance <= collectRange)
        {
            gem.Collect();
            return;
        }

        gem.MoveToward(transform.position, magnetSpeed);
    }
}
