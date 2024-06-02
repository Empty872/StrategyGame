using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    protected override int GetActionRange() => 5;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    public override string GetName() => "Shoot";
    private int _maxShootDistance = 6;
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => true;

    private float _stateTimer;
    private State _state;
    public Unit TargetUnit { get; private set; }
    public GridPosition TargetGridPosition { get; private set; }


    private enum State
    {
        Aiming,
        Shooting,
        CoolOff
    }
    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        Shoot();
    }

    private void Update()
    {
        if (!IsActive) return;
        _stateTimer -= Time.deltaTime;
        switch (_state)
        {
            case State.Aiming:
                var rotationSpeed = 10;
                var moveDirection = (TargetUnit.WorldPosition - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                PerformAction(TargetGridPosition);
                break;
            case State.CoolOff:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_stateTimer <= 0)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (_state)
        {
            case State.Aiming:
                _state = State.Shooting;
                var shootingStateTime = 0.1f;
                break;
            case State.Shooting:
                _state = State.CoolOff;
                var coolOffStateTime = 0.5f;
                _stateTimer = coolOffStateTime;
                break;
            case State.CoolOff:
                CompleteAction();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        TargetGridPosition = gridPosition;
        TargetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        _state = State.Aiming;
        var aimingStateTime = 1f;
        _stateTimer = aimingStateTime;
        StartAction(actionOnComplete);
    }


    private void Shoot()
    {
        TargetUnit.TakeDamage(10);
        OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = TargetUnit, shootingUnit = Unit });
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = TargetUnit, shootingUnit = Unit });
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionPriority = 100 + (1 - targetUnit.GetHealthNormalized()) * 100
        };
    }

    public override GridColorEnum GetColor() => GridColorEnum.Red;
    public override string GetDescription() => "Attack enemy from distance using ATK";
}