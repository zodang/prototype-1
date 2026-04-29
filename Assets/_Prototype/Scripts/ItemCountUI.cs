using TMPro;
using UnityEngine;

public class GemCountUI : MonoBehaviour
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
        DroppedGem.OnCollected += HandleGemCollected;
        SetCount(initialCount);
    }

    private void OnDisable()
    {
        DroppedGem.OnCollected -= HandleGemCollected;
    }

    private void HandleGemCollected(DroppedGem gem)
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
