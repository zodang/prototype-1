using System;
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
    
    [SerializeField] private float magnetRange = 1f;
    [SerializeField] private float magnetSpeed = 15f;
    
    [SerializeField] private float destroyRange = 0.1f;

    private Transform player;

    public void Init()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;
        
        StartCoroutine(ScatterRoutine());
        StartCoroutine(EnablePickupRoutine());
    }

    private void Update()
    {
        if (!canPickup || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < magnetRange)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );
        }
        
        if (dist <= destroyRange) Destroy(gameObject);
        
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