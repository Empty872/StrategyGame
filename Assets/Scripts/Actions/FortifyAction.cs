using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortifyAction : BaseAction
{
    public override string GetName() => "Fortify";
    protected override int GetActionRange() => 0;
    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnOneself() => true;
    protected override bool CanBeUsedOnEnemies() => false;
    protected override ActionRangeType GetActionRangeType() => ActionRangeType.Square;

    private int _maxDistance = 0;
    private int _extraDefense = 20;
    private int _effectDuration = 2;
    

    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        RiseDefense();
    }
    

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        StartAction(actionOnComplete);
        PerformAction(Unit.GridPosition);
        CompleteAction();
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
    public override string GetDescription() => "Increases DEF by " + _extraDefense + " for " + _effectDuration + " turns";

    public override int GetCooldown() => 3;
}