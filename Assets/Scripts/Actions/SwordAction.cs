using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnyAttack;
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    public override string GetName() => "Sword";
    protected override int GetActionRange() => 1;
    protected override ActionRangeType GetActionRangeType() => ActionRangeType.Square;
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => true;

    private int _maxAttackDistance = 1;
    private float _stateTimer;
    private State _state;
    public Unit TargetUnit { get; private set; }

    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        Attack();
    }

    private enum State
    {
        SwingingBeforeHit,
        SwingingAfterHit
    }

    private void Update()
    {
        if (!IsActive) return;
        _stateTimer -= Time.deltaTime;
        switch (_state)
        {
            case State.SwingingBeforeHit:
                var rotationSpeed = 10;
                var attackDirection = (TargetUnit.WorldPosition - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, attackDirection, rotationSpeed * Time.deltaTime);
                break;
            case State.SwingingAfterHit:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_stateTimer <= 0)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (_state)
        {
            case State.SwingingBeforeHit:
                _state = State.SwingingAfterHit;
                var afterHitStateTime = 0.5f;
                _stateTimer = afterHitStateTime;
                PerformAction(TargetUnit.GridPosition);
                break;
            case State.SwingingAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                CompleteAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TargetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        _state = State.SwingingBeforeHit;
        var beforeStateTime = 0.7f;
        _stateTimer = beforeStateTime;
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        StartAction(actionOnComplete);
    }

    private void Attack()
    {
        TargetUnit.TakeDamage(100);
        OnAnyAttack?.Invoke(this, EventArgs.Empty);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = 1000
        };
    }

    public override GridColorEnum GetColor() => GridColorEnum.Red;
    protected override float GetModifier() => 1;
    public override string GetDescription() => "Attack enemy in melee combat using ATK";
}