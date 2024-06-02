using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GrenadeAction : BaseAction
{
    protected override int GetActionRange() => 6;
    protected override bool CanBeUsedOnInteractableObjects() => true;
    protected override bool CanBeUsedOnEmpty() => true;
    protected override bool CanBeUsedOnOneself() => true;
    protected override bool CanBeUsedOnAllies() => true;
    protected override bool CanBeUsedOnEnemies() => true;
    public event EventHandler<OnThrowEventArgs> OnThrow;
    protected override int GetTargetRange() => 1;
    private List<GridPosition> _affectedGridPositions;
    private bool _canThrow = true;

    public class OnThrowEventArgs : EventArgs
    {
        public GridPosition targetGridPosition;

        public Unit shootingUnit;

        // public List<GridPosition> affectedGridPositions;
        public Action<GridPosition> onHitAffectAction;
    }

    public override string GetName() => "Grenade";

    private int _maxThrowDistance = 6;

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
    }

    private void Throw()
    {
        OnThrow?.Invoke(this,
            new OnThrowEventArgs
            {
                targetGridPosition = TargetGridPosition, shootingUnit = Unit,
                // affectedGridPositions = affectedGridPositionList, 
                onHitAffectAction = PerformAction
            });
        _canThrow = false;
    }

    protected override void AffectGridPosition(GridPosition targetGridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(targetGridPosition);
        var desctructible = LevelGrid.Instance.GetDestructibleAtGridPosition(targetGridPosition);
        if (unit is not null) unit.TakeDamage(GetFinalDamage(Unit.MagicAttack, unit.Defense));
        if (desctructible is not null) desctructible.Destruct();
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
    protected override float GetModifier() => 0.6f;
    public override string GetDescription() => "Deal MAG damage to all units in range";
}