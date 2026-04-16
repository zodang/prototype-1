using DG.Tweening;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    public float basicDuration = 0.05f;
    public float basicStrength = 0.15f;
    public int basicVibration = 5;
    
    private Vector3 originalPos;
    private Tween shakeTween;

    private void Awake()
    {
        originalPos = transform.localPosition;
    }
    
    public void PlayShake()
    {
        // 기존 흔들림 제거 (중첩 방지)
        shakeTween?.Kill();

        // 원래 위치 복구 보장
        transform.localPosition = originalPos;

        shakeTween = transform.DOShakePosition(
            basicDuration,
            basicStrength,
            basicVibration,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );
    }

    public void PlayShake(float duration, float strength, int vibrato)
    {
        // 기존 흔들림 제거 (중첩 방지)
        shakeTween?.Kill();

        // 원래 위치 복구 보장
        transform.localPosition = originalPos;

        shakeTween = transform.DOShakePosition(
            duration,
            strength,
            vibrato,
            randomness: 90,
            snapping: false,
            fadeOut: true
        );
    }
}