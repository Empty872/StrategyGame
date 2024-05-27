using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortifyAction : BaseAction
{
    public override string GetName() => "Fortify";
    private int _maxDistance = 0;
    private int _extraDefense = 20;
    private int _effectDuration = 2;

    private void Update()
    {
        if (!IsActive) return;
        CompleteAction();
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        RiseDefense();
        StartAction(actionOnComplete);
    }

    public override List<GridPosition> GetReachableActionGridPositionList()
    {
        var unitGridPosition = Unit.GridPosition;
        return new List<GridPosition> { unitGridPosition };
    }


    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetReachableActionGridPositionList();
    }


    private void RiseDefense()
    {
        Unit.ChangeDefense(_extraDefense);
        new DelayedAction(() => Unit.ChangeDefense(-_extraDefense), _effectDuration, !Unit.IsEnemy);
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
    public override string GetDescription() => "Increases DEF by " + _extraDefense;

    protected override int GetCooldown() => 3;
}