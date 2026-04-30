using UnityEngine;

public class BackgroundForwardEffect : MonoBehaviour
{
    [SerializeField] private Transform top;
    [SerializeField] private Transform front;
    [SerializeField] private Transform bottom;
    [SerializeField] private Transform back;

    [Header("Scroll")]
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float recycleY = -10f;
    [SerializeField] private Vector2 recyclePosition = new(0f, 20f);

    private Transform[] recycleOrder;
    private int recycleIndex;

    private void Awake()
    {
        ResolveLayerReferences();
        recycleOrder = new[] { bottom, front, top, back };
    }

    private void Update()
    {
        float moveAmount = scrollSpeed * Time.deltaTime;
        MoveDown(top, moveAmount);
        MoveDown(front, moveAmount);
        MoveDown(bottom, moveAmount);
        MoveDown(back, moveAmount);
        RecyclePassedLayers();
    }

    private void OnValidate()
    {
        scrollSpeed = Mathf.Max(0f, scrollSpeed);
    }

    private void MoveDown(Transform target, float moveAmount)
    {
        if (target == null)
        {
            return;
        }

        target.localPosition += Vector3.down * moveAmount;
    }

    private void RecyclePassedLayers()
    {
        if (recycleOrder == null || recycleOrder.Length == 0)
        {
            return;
        }

        for (int i = 0; i < recycleOrder.Length; i++)
        {
            Transform target = recycleOrder[recycleIndex];

            if (target == null)
            {
                AdvanceRecycleIndex();
                continue;
            }

            if (target.localPosition.y > recycleY)
            {
                return;
            }

            Vector3 position = target.localPosition;
            position.x = recyclePosition.x;
            position.y = recyclePosition.y;
            target.localPosition = position;
            AdvanceRecycleIndex();
        }
    }

    private void AdvanceRecycleIndex()
    {
        recycleIndex = (recycleIndex + 1) % recycleOrder.Length;
    }

    private void ResolveLayerReferences()
    {
        top ??= transform.Find("Top");
        front ??= transform.Find("Front");
        bottom ??= transform.Find("Bottom");
        back ??= transform.Find("Back");
    }
}
