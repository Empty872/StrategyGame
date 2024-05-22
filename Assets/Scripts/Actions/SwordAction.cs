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
    private int _maxAttackDistance = 1;
    private float _stateTimer;
    private State _state;
    public Unit TargetUnit { get; private set; }

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
                Attack();
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

    public override List<GridPosition> GetReachableActionGridPositionList()
    {
        var unitGridPosition = Unit.GridPosition;
        return GetReachableActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetReachableActionGridPositionList(GridPosition unitGridPosition)
    {
        var reachableGridPositionList = new List<GridPosition>();
        // var unitGridPosition = Unit.GridPosition;

        for (int x = -_maxAttackDistance; x <= _maxAttackDistance; x++)
        {
            for (int z = -_maxAttackDistance; z <= _maxAttackDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var possibleGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(possibleGridPosition))
                {
                    continue;
                }

                var possibleDistance = Mathf.Abs(x) + Mathf.Abs(z);
                // if (possibleDistance > _maxAttackDistance) continue;
                reachableGridPositionList.Add(possibleGridPosition);
            }
        }

        return reachableGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        var unitGridPosition = Unit.GridPosition;
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        var validGridPositionList = new List<GridPosition>();
        foreach (var gridPosition in GetReachableActionGridPositionList(unitGridPosition))
        {
            if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
            {
                // Grid Position is empty;
                continue;
            }

            // Both enemies in same team
            var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            if (targetUnit.IsEnemy == Unit.IsEnemy) continue;

            validGridPositionList.Add(gridPosition);
        }

        return validGridPositionList;
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

    public int GetTargetCountAtPosition(GridPosition unitGridPosition)
    {
        return GetValidActionGridPositionList(unitGridPosition).Count;
    }
}