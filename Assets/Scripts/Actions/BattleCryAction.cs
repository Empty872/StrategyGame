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
        StartFriendlyAction(this, gridPosition, PerformAction);
    }
    protected override void PerformAction(GridPosition targetGridPosition)
    {
        base.PerformAction(targetGridPosition);
        Invoke(nameof(CompleteAction), UnitAnimator.FriendlySpellCastAnimationTime);
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
        unit.AddBuff(new Buff(CharacteristicType.Attack, _extraAttack, _effectDuration, "Battle Cry",
            "Attack increased by " + _extraAttack));
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