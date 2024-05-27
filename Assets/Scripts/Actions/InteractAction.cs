using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int _maxInteractionDistance = 1;



    private void Update()
    {
        if (!IsActive) return;
    }

    public override void TakeAction(GridPosition targetGridPosition, Action actionOnComplete)
    {
        var interactable = LevelGrid.Instance.GetInteractableAtGridPosition(targetGridPosition);
        interactable.Interact(CompleteAction);
        StartAction(actionOnComplete);
    }

    public override List<GridPosition> GetReachableActionGridPositionList()
    {
        var reachableGridPositionList = new List<GridPosition>();
        var unitGridPosition = Unit.GridPosition;

        for (int x = -_maxInteractionDistance; x <= _maxInteractionDistance; x++)
        {
            for (int z = -_maxInteractionDistance; z <= _maxInteractionDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var possibleGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(possibleGridPosition))
                {
                    continue;
                }

                if (Unit.GridPosition == possibleGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }
                reachableGridPositionList.Add(possibleGridPosition);
            }
        }

        return reachableGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        var validGridPositionList = new List<GridPosition>();
        foreach (var gridPosition in GetReachableActionGridPositionList())
        {
            if (LevelGrid.Instance.HasInteractableAtGridPosition(gridPosition)) validGridPositionList.Add(gridPosition);
        }

        return validGridPositionList;
    }

    public override string GetName() => "Interact";

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionPriority = 0 };
    }
    public override GridColorEnum GetColor() => GridColorEnum.Yellow;
    public override int GetActionPointsCost() => 0;
    public override string GetDescription() => "Interact with object";

}