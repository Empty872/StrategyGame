using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushAction : BaseAction
{
    public override string GetName() => "Ambush";
    protected override int GetActionRange() => 0;
    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnOneself() => true;
    protected override bool CanBeUsedOnEnemies() => false;
    private int _effectDuration = 2;

    private void Update()
    {
        UpdateFriendlyCast();
    }


    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        Unit.AddBuff(new Buff(EnhanceShootAction, DisEnhanceShootAction, _effectDuration, GetName(),
            "Shoot attack increased in 3 times"));
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
        "Increases Shoot attack damage in 3 times on next turn";

    public override int GetCooldown() => 3;

    private void EnhanceShootAction()
    {
        GetComponent<ArrowShotAction>().EnhanceAttack();
    }

    private void DisEnhanceShootAction()
    {
        GetComponent<ArrowShotAction>().DisEnhanceAttack();
    }
}