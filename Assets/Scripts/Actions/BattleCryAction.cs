using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCryAction : BaseAction
{
    protected override int GetCooldown() => 4;
    private int _extraAttack = 10;
    private int _effectDuration = 2;
    public override string GetName() => "Battle Cry";
    private int _maxActionDistance = 1;

    private void Update()
    {
        if (!IsActive) return;
        AffectGridPositions(GetValidActionGridPositionList());
    }


    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
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

        for (int x = -_maxActionDistance; x <= _maxActionDistance; x++)
        {
            for (int z = -_maxActionDistance; z <= _maxActionDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var possibleGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(possibleGridPosition))
                {
                    continue;
                }

                reachableGridPositionList.Add(possibleGridPosition);
            }
        }

        return reachableGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetReachableActionGridPositionList();
    }

    private void AffectGridPositions(List<GridPosition> gridPositions)
    {
        foreach (var gridPosition in gridPositions)
        {
            AffectGridPosition(gridPosition);
        }

        CompleteAction();
    }

    private void AffectGridPosition(GridPosition targetGridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        ;
        if (unit is not null && unit.IsEnemy == Unit.IsEnemy)
        {
            RiseAttack(unit);
        }

        ;
    }

    private void RiseAttack(Unit unit)
    {
        unit.ChangeAttack(_extraAttack);
        new DelayedAction(() => { unit.ChangeAttack(-_extraAttack); }, _effectDuration, !Unit.IsEnemy);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = 0
        };
    }

    public override GridColorEnum GetColor() => GridColorEnum.Green;
    public override string GetDescription() => "Increases allies' ATK by " + _extraAttack;

    public override List<GridPosition> GetAffectedGridPositionList(GridPosition targetGridPosition)
    {
        var reachableGridPositionList = GetReachableActionGridPositionList();
        if (!reachableGridPositionList.Contains(targetGridPosition)) return new List<GridPosition>();
        return GetReachableActionGridPositionList();
    }
}