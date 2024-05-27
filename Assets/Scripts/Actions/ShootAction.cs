using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    public override string GetName() => "Shoot";
    private int _maxShootDistance = 6;
    private float _stateTimer;
    private State _state;
    public Unit TargetUnit { get; private set; }


    private enum State
    {
        Aiming,
        Shooting,
        CoolOff
    }

    private void Update()
    {
        if (!IsActive) return;
        _stateTimer -= Time.deltaTime;
        switch (_state)
        {
            case State.Aiming:
                var rotationSpeed = 10;
                var moveDirection = (TargetUnit.WorldPosition - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                Shoot();

                break;
            case State.CoolOff:
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
            case State.Aiming:
                _state = State.Shooting;
                var shootingStateTime = 0.1f;
                break;
            case State.Shooting:
                _state = State.CoolOff;
                var coolOffStateTime = 0.5f;
                _stateTimer = coolOffStateTime;
                break;
            case State.CoolOff:
                CompleteAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TargetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        _state = State.Aiming;
        var aimingStateTime = 1f;
        _stateTimer = aimingStateTime;
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

        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++)
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var possibleGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(possibleGridPosition))
                {
                    continue;
                }

                var possibleDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (possibleDistance > _maxShootDistance) continue;
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

    private void Shoot()
    {
        var finalDamage = GetFinalDamage(Unit.Attack, TargetUnit.Defense);
        TargetUnit.TakeDamage(finalDamage);
        OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = TargetUnit, shootingUnit = Unit });
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = TargetUnit, shootingUnit = Unit });
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = 100 + (1 - targetUnit.GetHealthNormalized()) * 100
        };
    }

    public int GetTargetCountAtPosition(GridPosition unitGridPosition)
    {
        return GetValidActionGridPositionList(unitGridPosition).Count;
    }

    public override GridColorEnum GetColor() => GridColorEnum.Red;
    public override string GetDescription() => "Attack enemy from distance using ATK";
}