using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IceBoltAction : BaseAction
{
    protected override int GetActionRange() => 4;
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => true;
    protected override float GetModifier() => 0.8f;
    public event EventHandler<OnHostileBaseActionEventArgs> OnThrow;
    private bool _canThrow = true;
    private int _slowDownAmount = 1;
    private int _effectDuration = 2;

    public override string GetName() => "Ice Bolt";

    // private float _stateTimer;
    // private State _state;
    public GridPosition TargetGridPosition { get; private set; }

    // private bool _canShoot = true;


    private void Update()
    {
        if (!IsActive) return;
        if (!_canThrow) return;
        Throw();
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
        _canThrow = true;
    }

    private void Throw()
    {
        OnThrow?.Invoke(this,
            new OnHostileBaseActionEventArgs
            {
                targetGridPosition = TargetGridPosition, unit = Unit,
                // affectedGridPositions = affectedGridPositionList, 
                actionOnCastFinished = PerformAction
            });
        _canThrow = false;
    }

    protected override void AffectGridPosition(GridPosition targetGridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        if (unit is not null)
        {
            unit.TakeDamage(GetFinalDamage(Unit.MagicAttack, unit.Defense));
            unit.AddBuff(new Buff(CharacteristicType.Speed, -_slowDownAmount, _effectDuration, GetName(), "Slowed down by 1"));
        }
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = 0
        };
    }

    public override int GetCooldown() => 2;

    public override GridColorEnum GetColor() => GridColorEnum.Red;
    public override string GetDescription() => "Deal MAG and slow down enemy";
}