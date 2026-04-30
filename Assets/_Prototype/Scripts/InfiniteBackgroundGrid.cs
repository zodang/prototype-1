using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackgroundGrid : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float tileSize = 10f;
    [SerializeField] private int columns = 3;
    [SerializeField] private int rows = 3;

    private readonly List<Transform> tiles = new();

    private float Width => tileSize * columns;
    private float Height => tileSize * rows;
    private float HorizontalThreshold => Width * 0.5f;
    private float VerticalThreshold => Height * 0.5f;

    private void Awake()
    {
        ResolveTarget();
        CacheTiles();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            ResolveTarget();
        }

        if (target == null || tiles.Count == 0)
        {
            return;
        }

        Vector3 targetLocalPosition = transform.InverseTransformPoint(target.position);

        foreach (Transform tile in tiles)
        {
            RecycleTile(tile, targetLocalPosition);
        }
    }

    private void OnValidate()
    {
        tileSize = Mathf.Max(0.01f, tileSize);
        columns = Mathf.Max(1, columns);
        rows = Mathf.Max(1, rows);
    }

    private void RecycleTile(Transform tile, Vector3 targetLocalPosition)
    {
        Vector3 position = tile.localPosition;

        while (targetLocalPosition.x - position.x > HorizontalThreshold)
        {
            position.x += Width;
        }

        while (position.x - targetLocalPosition.x > HorizontalThreshold)
        {
            position.x -= Width;
        }

        while (targetLocalPosition.y - position.y > VerticalThreshold)
        {
            position.y += Height;
        }

        while (position.y - targetLocalPosition.y > VerticalThreshold)
        {
            position.y -= Height;
        }

        tile.localPosition = position;
    }

    private void CacheTiles()
    {
        tiles.Clear();

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<SpriteRenderer>(out _))
            {
                tiles.Add(child);
            }
        }
    }

    private void ResolveTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        target = player != null ? player.transform : target;
    }
}
