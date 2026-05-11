using System;
using System.Collections;
using UnityEngine;

/*
체인 공격 자원 게이지 관리
- 자원 소모 및 이벤트 실행
- 재장전
*/

public class ChainAttackResource
{
    private readonly MonoBehaviour _coroutineOwner;
    private readonly float _emptyDisplaySeconds;
    private readonly float _reloadSeconds;
    private readonly Action _onReloadStarted;

    private Coroutine _reloadCoroutine;
    private float _decreaseTimer;
    private float _currentAmount;

    public ChainAttackResource(
        MonoBehaviour coroutineOwner,
        float maxAmount,
        float emptyDisplaySeconds,
        float reloadSeconds,
        Action onReloadStarted)
    {
        _coroutineOwner = coroutineOwner;
        _emptyDisplaySeconds = Mathf.Max(0f, emptyDisplaySeconds);
        _reloadSeconds = Mathf.Max(0f, reloadSeconds);
        _onReloadStarted = onReloadStarted;
        MaxAmount = Mathf.Max(0f, maxAmount);
        _currentAmount = MaxAmount;
    }

    public float MaxAmount { get; }
    public float CurrentAmount => _currentAmount;
    public float NormalizedAmount => MaxAmount <= 0f ? 0f : Mathf.Clamp01(_currentAmount / MaxAmount);
    public bool IsReloading => _reloadCoroutine != null;
    public bool CanUse => !IsReloading && _currentAmount > 0f;
    public event Action<float, float> OnAmountChanged;

    public void NotifyInitialAmount()
    {
        NotifyAmountChanged();
    }

    public bool TrySpendInitialCost()
    {
        if (!CanUse)
        {
            return false;
        }

        SpendOne();
        return CanUse;
    }

    public void SpendOverTime(float deltaTime, float amountPerSecond)
    {
        if (_currentAmount <= 0f || amountPerSecond <= 0f)
        {
            if (_currentAmount <= 0f)
            {
                StartReload();
            }

            return;
        }

        _decreaseTimer += deltaTime;
        float decreaseInterval = 1f / amountPerSecond;

        while (_decreaseTimer >= decreaseInterval && _currentAmount > 0f)
        {
            _decreaseTimer -= decreaseInterval;
            SpendOne();
        }
    }

    public void ResetSpendTimer()
    {
        _decreaseTimer = 0f;
    }

    private void SpendOne()
    {
        _currentAmount = Mathf.Max(0f, _currentAmount - 1f);
        NotifyAmountChanged();

        if (_currentAmount <= 0f)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        if (_reloadCoroutine != null)
        {
            return;
        }

        _onReloadStarted?.Invoke();
        _decreaseTimer = 0f;
        _currentAmount = 0f;
        NotifyAmountChanged();
        _reloadCoroutine = _coroutineOwner.StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (_emptyDisplaySeconds > 0f)
        {
            yield return new WaitForSeconds(_emptyDisplaySeconds);
        }
        else
        {
            yield return null;
        }

        if (_reloadSeconds <= 0f)
        {
            _currentAmount = MaxAmount;
            _reloadCoroutine = null;
            NotifyAmountChanged();
            yield break;
        }

        int reloadStepCount = Mathf.CeilToInt(MaxAmount);
        if (reloadStepCount <= 0)
        {
            _currentAmount = 0f;
            _reloadCoroutine = null;
            NotifyAmountChanged();
            yield break;
        }

        float reloadInterval = _reloadSeconds / reloadStepCount;
        while (_currentAmount < MaxAmount)
        {
            yield return new WaitForSeconds(reloadInterval);
            _currentAmount = Mathf.Min(MaxAmount, _currentAmount + 1f);
            NotifyAmountChanged();
        }

        _reloadCoroutine = null;
    }

    private void NotifyAmountChanged()
    {
        OnAmountChanged?.Invoke(_currentAmount, MaxAmount);
    }
}
