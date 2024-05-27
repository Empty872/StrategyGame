using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GrenadeAction : BaseAction
{
    public static event EventHandler<OnThrowEventArgs> OnAnyThrow;
    public event EventHandler<OnThrowEventArgs> OnThrow;
    private int _explosionRadius = 1;
    private List<GridPosition> _affectedGridPositions;
    private bool _canThrow = true;

    public class OnThrowEventArgs : EventArgs
    {
        public GridPosition targetGridPosition;
        public Unit shootingUnit;
        public List<GridPosition> affectedGridPositions;
        public Action<List<GridPosition>> onHitAffectAction;
    }
    
    public override string GetName() => "Grenade";

    private int _maxThrowDistance = 6;

    // private float _stateTimer;
    // private State _state;
    public GridPosition TargetGridPosition { get; private set; }

    // private bool _canShoot = true;


    private void Update()
    {
        if (!IsActive) return;
        if (!_canThrow) return;
        Throw();
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TargetGridPosition = gridPosition;
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

        for (int x = -_maxThrowDistance; x <= _maxThrowDistance; x++)
        {
            for (int z = -_maxThrowDistance; z <= _maxThrowDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var possibleGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(possibleGridPosition))
                {
                    continue;
                }

                var possibleDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (possibleDistance > _maxThrowDistance) continue;
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
        return GetReachableActionGridPositionList(unitGridPosition);
    }

    private void Throw()
    {
        var affectedGridPositionList = GetAffectedGridPositionList(TargetGridPosition);
        OnThrow?.Invoke(this,
            new OnThrowEventArgs
            {
                targetGridPosition = TargetGridPosition, shootingUnit = Unit,
                affectedGridPositions = affectedGridPositionList, onHitAffectAction = AffectGridPositions
            });
        OnAnyThrow?.Invoke(this, new OnThrowEventArgs { targetGridPosition = TargetGridPosition, shootingUnit = Unit });
        _canThrow = false;
    }

    public override List<GridPosition> GetAffectedGridPositionList(GridPosition targetGridPosition)
    {
        var affectedGridPositionList = new List<GridPosition>();
        if (!LevelGrid.Instance.IsPossibleGridPosition(targetGridPosition, this)) return affectedGridPositionList;
        for (var x = -_explosionRadius; x < _explosionRadius + 1; x++)
        {
            for (var z = -_explosionRadius; z < _explosionRadius + 1; z++)
            {
                var gridPosition = targetGridPosition + new GridPosition(x, z);

                if (LevelGrid.Instance.IsValidGridPosition(gridPosition))
                    affectedGridPositionList.Add(gridPosition);
            }
        }

        return affectedGridPositionList;
    }

    private void AffectGridPosition(GridPosition targetGridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        var desctructible = LevelGrid.Instance.GetDestructibleAtGridPosition(targetGridPosition);
        if (unit is not null)
        {
            var finalDamage = GetFinalDamage(Unit.MagicAttack, unit.Defense);
            unit.TakeDamage(finalDamage);
        };
        if (desctructible is not null) desctructible.Destruct();
    }

    private void AffectGridPositions(List<GridPosition> gridPositions)
    {
        foreach (var gridPosition in gridPositions)
        {
            AffectGridPosition(gridPosition);
        }
        CompleteAction();
        _canThrow = true;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        // var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = -1
        };
    }

    private void DealDamage()
    {
    }

    public int GetTargetCountAtPosition(GridPosition unitGridPosition)
    {
        return GetValidActionGridPositionList(unitGridPosition).Count;
    }

    protected override int GetCooldown() => 2;
    public override GridColorEnum GetColor() => GridColorEnum.Red;
    protected override float GetModifier() => 0.6f;
    public override string GetDescription() => "Deal MAG damage to all units in range";
}