using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAction : BaseAction
{
    // public override int GetCooldown() => 1;

    public int HealAmount => (int)(Unit.MagicAttack * 0.7f);
    public override string GetName() => "Heal";
    protected override int GetActionRange() => 1;
    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnEnemies() => false;
    protected override ActionRangeType GetActionRangeType() => ActionRangeType.Square;
    public override int GetCooldown() => 1;

    public Unit TargetUnit { get; private set; }


    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        Heal();
    }
// изза кулдауна перс не может окончить действие        

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TargetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        StartAction(actionOnComplete);
        PerformAction(gridPosition);
        CompleteAction();
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

    public override GridColorEnum GetColor() => GridColorEnum.Green;
    public override string GetDescription() => "Heal one ally by " + HealAmount + " HP";
}