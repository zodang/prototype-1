using System;
using System.Collections;
using UnityEngine;

/*
체인 공격의 전체 흐름 조율
- 공격 관련 수치 관리
- 입력 기반 공격 시작/종료
- 공격 루프 실행
- 각 시스템 호출 순서 관리 (ChainLineRendererView, ChainTargetSelector, ChainAttackResources)
*/

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CameraEffect cameraEffect;

    [Header("Attack")]
    [SerializeField] private int attackRadius = 3;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private float attackInterval = 1.0f;

    [Header("Chain")]
    [SerializeField] private int maxChainCount = 3;
    [SerializeField] private int maxChainBranchCount = 1;
    [SerializeField] private int detectionBufferSize = 16;
    [SerializeField] private float chainAttackHoldSeconds = 0.15f;

    [Header("Chain Attack Resource")]
    [SerializeField] private float maxChainAttackAmount = 5f;
    [SerializeField] private float chainAttackDecreasePerSecond = 1f;
    [SerializeField] private float chainAttackReloadSeconds = 3f;
    [SerializeField] private float chainAttackEmptyDisplaySeconds = 0.15f;

    private ChainAttackResource _chainAttackResource;
    private ChainTargetSelector _targetSelector;
    private ChainLineRendererView _lineView;
    private Coroutine _attackCoroutine;
    private float _chainAttackHoldTimer;

    public float MaxChainAttackAmount => _chainAttackResource?.MaxAmount ?? maxChainAttackAmount;
    public float CurrentChainAttackAmount => _chainAttackResource?.CurrentAmount ?? maxChainAttackAmount;
    public float NormalizedChainAttackAmount => _chainAttackResource?.NormalizedAmount ?? 1f;
    public bool IsChainAttackReloading => _chainAttackResource?.IsReloading ?? false;
    public bool CanUseChainAttack => _chainAttackResource?.CanUse ?? false;
    public event Action<float, float> OnChainAttackAmountChanged;

    private void OnValidate()
    {
        attackRadius = Mathf.Max(0, attackRadius);
        attackDamage = Mathf.Max(0f, attackDamage);
        attackInterval = Mathf.Max(0.01f, attackInterval);
        maxChainCount = Mathf.Max(1, maxChainCount);
        maxChainBranchCount = Mathf.Max(1, maxChainBranchCount);
        detectionBufferSize = Mathf.Max(1, detectionBufferSize);
        chainAttackHoldSeconds = Mathf.Max(0f, chainAttackHoldSeconds);
        maxChainAttackAmount = Mathf.Max(0f, maxChainAttackAmount);
        chainAttackDecreasePerSecond = Mathf.Max(0f, chainAttackDecreasePerSecond);
        chainAttackReloadSeconds = Mathf.Max(0f, chainAttackReloadSeconds);
        chainAttackEmptyDisplaySeconds = Mathf.Max(0f, chainAttackEmptyDisplaySeconds);
    }

    private void Start()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        _lineView = new ChainLineRendererView(transform, lineRenderer);
        _targetSelector = new ChainTargetSelector(detectionBufferSize);
        _chainAttackResource = new ChainAttackResource(
            this,
            maxChainAttackAmount,
            chainAttackEmptyDisplaySeconds,
            chainAttackReloadSeconds,
            HandleReloadStarted);

        _chainAttackResource.OnAmountChanged += NotifyChainAttackAmountChanged;
        _chainAttackResource.NotifyInitialAmount();
    }

    private void OnDestroy()
    {
        if (_chainAttackResource != null)
        {
            _chainAttackResource.OnAmountChanged -= NotifyChainAttackAmountChanged;
        }

        _targetSelector?.Clear();
    }

    public void IncreaseMaxChainBranchCount()
    {
        maxChainBranchCount++;
    }

    public void IncreaseMaxChainCount()
    {
        maxChainCount++;
    }

    private void Update()
    {
        _targetSelector.UpdateTargets(transform.position, attackRadius, maxChainCount, maxChainBranchCount);

        if (inputManager.IsTryingToChainAttack && CanUseChainAttack)
        {
            _chainAttackHoldTimer += Time.deltaTime;
            if (_chainAttackHoldTimer < chainAttackHoldSeconds)
            {
                HandleAttackStop();
                return;
            }

            HandleAttackStart();
            _chainAttackResource.SpendOverTime(Time.deltaTime, chainAttackDecreasePerSecond);
            _lineView.Draw(transform.position, _targetSelector.Branches, attackRadius);
        }
        else
        {
            _chainAttackHoldTimer = 0f;
            HandleAttackStop();
        }
    }

    private void HandleAttackStart()
    {
        if (_attackCoroutine != null || !CanUseChainAttack)
        {
            return;
        }

        _chainAttackResource.ResetSpendTimer();
        if (!_chainAttackResource.TrySpendInitialCost())
        {
            return;
        }

        _attackCoroutine = StartCoroutine(AttackRoutine());
        _lineView.Draw(transform.position, _targetSelector.Branches, attackRadius);
    }

    private void HandleAttackStop()
    {
        StopAttack();
        _lineView.Clear();
    }

    private void HandleReloadStarted()
    {
        HandleAttackStop();
    }

    private void StopAttack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }

        _chainAttackResource?.ResetSpendTimer();
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            DamageCurrentChain();
            yield return new WaitForSeconds(attackInterval);
        }
    }

    private void DamageCurrentChain()
    {
        bool damagedAnyEnemy = false;
        for (int i = _targetSelector.Chain.Count - 1; i >= 0; i--)
        {
            Enemy enemy = _targetSelector.Chain[i];
            if (enemy == null)
            {
                _targetSelector.RemoveFromChain(enemy);
                continue;
            }

            enemy.TryDamage(attackDamage);
            damagedAnyEnemy = true;
        }

        if (damagedAnyEnemy)
        {
            cameraEffect.PlayShake();
        }
    }

    private void NotifyChainAttackAmountChanged(float currentAmount, float maxAmount)
    {
        OnChainAttackAmountChanged?.Invoke(currentAmount, maxAmount);
    }
}
