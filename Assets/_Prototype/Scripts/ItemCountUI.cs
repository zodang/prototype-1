using TMPro;
using UnityEngine;

public class ItemCountUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countText;
    [SerializeField] private int initialCount = 0;

    private int count;

    private void Awake()
    {
        if (countText == null)
        {
            countText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        DroppedItem.OnCollected += HandleItemCollected;
        SetCount(initialCount);
    }

    private void OnDisable()
    {
        DroppedItem.OnCollected -= HandleItemCollected;
    }

    private void HandleItemCollected(DroppedItem item)
    {
        SetCount(count + 1);
    }

    private void SetCount(int value)
    {
        count = Mathf.Max(0, value);

        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }
}
