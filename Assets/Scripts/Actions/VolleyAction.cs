using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VolleyAction : BaseAction
{
    protected override int GetActionRange() => 3;
    protected override bool CanBeUsedOnInteractableObjects() => true;
    protected override bool CanBeUsedOnEmpty() => true;
    protected override bool CanBeUsedOnOneself() => true;
    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnEnemies() => true;
    protected override int GetTargetRange() => 1;
    private bool _canShoot = true;


    public override string GetName() => "Volley";
    public GridPosition TargetGridPosition { get; private set; }


    private void Update()
    {
        if (!IsActive) return;
        if (!_canShoot) return;
        Shoot();
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TargetGridPosition = gridPosition;
        StartAction(actionOnComplete);
    }

    protected override void PerformAction(GridPosition targetGridPosition)
    {
        base.PerformAction(targetGridPosition);
        CompleteAction();
        _canShoot = true;
    }

    private void Shoot()
    {
        _canShoot = false;
        PerformAction(TargetGridPosition);
    }

    protected override void AffectGridPosition(GridPosition targetGridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        if (unit is not null && unit.IsEnemy != Unit.IsEnemy)
            unit.TakeDamage(GetFinalDamage(Unit.Attack, unit.Defense));
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        // var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = 0
        };
    }

    public override int GetCooldown() => 2;

    public override GridColorEnum GetColor() => GridColorEnum.Red;

    protected override float GetModifier() => 0.8f;
    public override string GetDescription() => "Deal ATK to enemies in range";
}