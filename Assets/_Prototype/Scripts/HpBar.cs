using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Transform instantFill;
    [SerializeField] private Transform delayedFill;

    [SerializeField] private float delayedSpeed = 2f;
    [SerializeField] private float delayTime = 0.3f;

    private float targetRatio = 1f;
    private float delayedRatio = 1f;

    private float delayTimer = 0f;

    private Vector3 instantOriginalScale;
    private Vector3 delayedOriginalScale;

    private void Awake()
    {
        instantOriginalScale = instantFill.localScale;
        delayedOriginalScale = delayedFill.localScale;
    }

    public void SetHP(float current, float max)
    {
        targetRatio = Mathf.Clamp01(current / max);

        // 즉시 반영
        instantFill.localScale = new Vector3(
            instantOriginalScale.x * targetRatio,
            instantOriginalScale.y,
            instantOriginalScale.z
        );

        delayTimer = 0f;
    }

    private void Update()
    {
        if (delayTimer < delayTime)
        {
            delayTimer += Time.deltaTime;
            return;
        }

        delayedRatio = Mathf.Lerp(delayedRatio, targetRatio, Time.deltaTime * delayedSpeed);

        delayedFill.localScale = new Vector3(
            delayedOriginalScale.x * delayedRatio,
            delayedOriginalScale.y,
            delayedOriginalScale.z
        );
    }
}