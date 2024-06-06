using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerAction : BaseAction
{
    public override string GetName() => "Prayer";
    protected override int GetActionRange() => 0;
    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnOneself() => true;
    protected override bool CanBeUsedOnEnemies() => false;
    protected override ActionRangeType GetActionRangeType() => ActionRangeType.Square;
    private int _extraDefense = 10;
    private int _extraMagic = 10;
    private int _effectDuration = 3;


    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        Unit.AddBuff(new Buff(CharacteristicType.Defense, _extraDefense, _effectDuration, "Prayer Defense",
            "Defence increased by " + _extraDefense));
        Unit.AddBuff(new Buff(CharacteristicType.MagicAttack, _extraMagic, _effectDuration, "Prayer Power",
            "Magic increased by " + _extraMagic));
    }

    private void Update()
    {
        UpdateFriendlyCast();
    }
    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TakeFriendlyCastAction(gridPosition);
        StartFriendlyCastAction(this, gridPosition, actionOnComplete);
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

    public override string GetDescription() =>
        "Increases DEF and MAG by " + _extraDefense + " for " + _effectDuration + " turns";

    public override int GetCooldown() => 5;
}