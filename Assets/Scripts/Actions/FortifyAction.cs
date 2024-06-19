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

    private int _extraDefense = 20;
    private int _effectDuration = 2;


    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        RiseDefense();
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


    private void RiseDefense()
    {
        Unit.AddBuff(new Buff(CharacteristicType.Defense, _extraDefense, _effectDuration, "Fortify",
            "Defence increased by " + _extraDefense));
    }

    public override GridColorEnum GetColor() => GridColorEnum.Green;

    public override string GetDescription() =>
        "Increases DEF by " + _extraDefense + " for " + _effectDuration + " turns";

    public override int GetCooldown() => 3;
}