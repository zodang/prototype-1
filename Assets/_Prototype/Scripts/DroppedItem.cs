using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class DroppedItem : MonoBehaviour
{
    [Header("Scatter")]
    [SerializeField] private float scatterMaxDistance = 1f;
    [SerializeField] private float scatterMinDistance = 0.6f;
    [SerializeField] private float scatterDuration = 0.5f;

    [Header("Pickup")]
    [SerializeField] private float pickupDelay = 0.5f;

    private bool canPickup = false;

    public bool CanPickup => canPickup;

    public void Init()
    {
        StartCoroutine(ScatterRoutine());
        StartCoroutine(EnablePickupRoutine());
    }

    public void MoveToward(Vector3 targetPosition, float speed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    public void Collect()
    {
        Destroy(gameObject);
    }

    private IEnumerator ScatterRoutine()
    {
        Vector3 start = transform.position;

        Vector2 dir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(scatterMinDistance, scatterMaxDistance);

        Vector3 target = start + (Vector3)(dir * dist);

        float time = 0f;

        while (time < scatterDuration)
        {
            time += Time.deltaTime;
            float t = time / scatterDuration;

            // Ease Out (핵심)
            t = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(start, target, t);

            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator EnablePickupRoutine()
    {
        canPickup = false;
        yield return new WaitForSeconds(pickupDelay);
        canPickup = true;
    }
}
