using DG.Tweening;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform followTarget;
    [SerializeField] private float followSmoothTime = 0.15f;
    [SerializeField] private bool useInitialOffset = true;
    [SerializeField] private Vector3 followOffset;

    [Header("Shake")]
    public float basicDuration = 0.05f;
    public float basicStrength = 0.15f;
    public int basicVibration = 5;

    private Vector3 followPosition;
    private Vector3 followVelocity;
    private Vector3 shakeOffset;
    private Tween shakeTween;

    private void Awake()
    {
        followPosition = transform.position;

        if (followTarget != null && useInitialOffset)
        {
            followOffset = transform.position - followTarget.position;
        }
    }

    private void LateUpdate()
    {
        if (followTarget != null)
        {
            Vector3 targetPosition = followTarget.position + followOffset;
            followPosition = Vector3.SmoothDamp(
                followPosition,
                targetPosition,
                ref followVelocity,
                followSmoothTime
            );
        }
        else
        {
            followPosition = transform.position - shakeOffset;
        }

        transform.position = followPosition + shakeOffset;
    }

    public void PlayShake()
    {
        shakeTween?.Kill();
        shakeOffset = Vector3.zero;

        shakeTween = CreateShakeTween(basicDuration, basicStrength, basicVibration);
    }

    public void PlayShake(float duration, float strength, int vibrato)
    {
        shakeTween?.Kill();
        shakeOffset = Vector3.zero;

        shakeTween = CreateShakeTween(duration, strength, vibrato);
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;

        if (followTarget != null && useInitialOffset)
        {
            followOffset = transform.position - shakeOffset - followTarget.position;
        }
    }

    private Tween CreateShakeTween(float duration, float strength, int vibrato)
    {
        return DOTween.Shake(
            () => shakeOffset,
            value => shakeOffset = value,
            duration,
            strength,
            vibrato,
            randomness: 90,
            ignoreZAxis: true,
            fadeOut: true
        ).OnComplete(() => shakeOffset = Vector3.zero);
    }
}
