using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(DelayedDestroy), 5.0f);
    }

    private void DelayedDestroy()
    {
        Destroy(gameObject);
    }
}
