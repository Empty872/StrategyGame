using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : BaseAction
{
    public override int GetCooldown() => 1;

    public int HealAmount => (int)(Unit.MagicAttack * 0.7f);
    public override string GetName() => "Heal";
    private int _maxActionDistance = 1;
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
        CompleteAction();
    }


    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TargetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        Heal();
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
            if (targetUnit.IsEnemy != Unit.IsEnemy) continue;

            validGridPositionList.Add(gridPosition);
        }

        return validGridPositionList;
    }

    private void Heal()
    {
        TargetUnit.RestoreHealth(HealAmount);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = 0
        };
    }

    public int GetTargetCountAtPosition(GridPosition unitGridPosition)
    {
        return GetValidActionGridPositionList(unitGridPosition).Count;
    }
    public override GridColorEnum GetColor() => GridColorEnum.Green;
    public override string GetDescription() => "Heal one ally by " + HealAmount + " HP";

}