using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCryAction : BaseAction
{
    public override int GetCooldown() => 4;
    private int _extraAttack = 10;
    private int _effectDuration = 2;
    public override string GetName() => "Battle Cry";
    protected override int GetTargetRange() => 1;
    private int _maxActionDistance = 1;


    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        StartAction(actionOnComplete);
        PerformAction(Unit.GridPosition);
        CompleteAction();
    }


    protected override void AffectGridPosition(GridPosition targetGridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        if (unit is not null && unit.IsEnemy == Unit.IsEnemy)
        {
            RiseAttack(unit);
        }
    }
    public override List<GridPosition> GetAffectedGridPositionList(GridPosition gridPosition)
    {
        if (!LevelGrid.Instance.IsReachablePosition(gridPosition, this)) return new List<GridPosition>();
        return base.GetAffectedGridPositionList(Unit.GridPosition);
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
    protected override int GetActionRange() => 1;

    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnEnemies() => true;
    protected override bool CanBeUsedOnOneself() => true;
    protected override bool CanBeUsedOnEmpty() => true;
    protected override ActionRangeType GetActionRangeType() => ActionRangeType.Square;

    public override string GetDescription() =>
        "Increases allies' ATK by " + _extraAttack + " for " + _effectDuration + " turns";
    
}