using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    None,
    Absorb,
    Spit
}

public class PlayerAbsorption : MonoBehaviour
{
    public PlayerState PlayerState = PlayerState.None;

    [SerializeField] private float absorbRange = 5f;
    [SerializeField] private float absorbDamagePerSecond = 5f;
    [SerializeField] private float absorbAngle = 90f;
    [SerializeField] private LayerMask enemyLayer;

    private Vector2 _lookDirection = Vector2.right;

    public void SetLookDirection(Vector2 dir)
    {
        if (dir != Vector2.zero)
            _lookDirection = dir.normalized;
    }

    public void OnAbsorb(InputValue value)
    {
        PlayerState = value.Get<float>() == 1 ? PlayerState.Absorb : PlayerState.None;
    }

    private void Update()
    {
        Absorb();

        DrawDebugFan();
    }

    private void Absorb()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            absorbRange,
            enemyLayer
        );

        float halfAngle = absorbAngle * 0.5f;

        foreach (Collider2D hit in hits)
        {
            Vector2 toTarget = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;

            float angle = Vector2.Angle(_lookDirection, toTarget);

            if (angle <= halfAngle)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(absorbDamagePerSecond * Time.deltaTime);
                }
            }
        }
    }

    private void DrawDebugFan()
    {
        Vector3 origin = transform.position;

        float halfAngle = absorbAngle * 0.5f;

        Vector2 leftDir = Rotate(_lookDirection, -halfAngle);
        Vector2 rightDir = Rotate(_lookDirection, halfAngle);

        Debug.DrawLine(origin, origin + (Vector3)(_lookDirection * absorbRange), Color.red);
        Debug.DrawLine(origin, origin + (Vector3)(leftDir * absorbRange), Color.yellow);
        Debug.DrawLine(origin, origin + (Vector3)(rightDir * absorbRange), Color.yellow);

        int segmentCount = 20;
        Vector3 prevPoint = origin + (Vector3)(leftDir * absorbRange);

        for (int i = 1; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            Vector2 dir = Rotate(_lookDirection, currentAngle);
            Vector3 nextPoint = origin + (Vector3)(dir * absorbRange);

            Debug.DrawLine(prevPoint, nextPoint, Color.cyan);
            prevPoint = nextPoint;
        }
    }

    private Vector2 Rotate(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        ).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, absorbRange);
    }
}