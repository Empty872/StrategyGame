using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassHealAction : BaseAction
{
    private int HealAmount => (int)(Unit.MagicAttack * GetModifier());
    public override int GetCooldown() => 3;
    public override string GetName() => "Mass Heal";
    protected override int GetTargetRange() => 2;
    protected override float GetModifier() => 0.3f;
    private int _maxActionDistance = 1;

    private void Update()
    {
        UpdateFriendlyCast();
    }
    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TakeFriendlyCastAction(gridPosition);
        StartFriendlyCastAction(this, gridPosition, actionOnComplete);
    }


    protected override void AffectGridPosition(GridPosition targetGridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        if (unit is not null && unit.IsEnemy == Unit.IsEnemy)
        {
            Heal(unit);
        }
    }

    private void Heal(Unit unit)
    {
        unit.RestoreHealth(HealAmount);
    }

    public override List<GridPosition> GetAffectedGridPositionList(GridPosition gridPosition)
    {
        if (!LevelGrid.Instance.IsReachablePosition(gridPosition, this)) return new List<GridPosition>();
        return base.GetReachableActionGridPositionList();
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
    protected override int GetActionRange() => 2;

    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnEnemies() => true;
    protected override bool CanBeUsedOnOneself() => true;
    protected override bool CanBeUsedOnEmpty() => true;

    public override string GetDescription() =>
        "Heal allies by " + HealAmount + " HP";
}