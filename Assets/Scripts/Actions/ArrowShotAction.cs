using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShotAction : BaseAction
{
    protected override int GetActionRange() => 3;

    public static event EventHandler<OnHostileBaseActionEventArgs> OnAnyShoot;
    public event EventHandler<OnHostileBaseActionEventArgs> OnShot;
    public event EventHandler<OnHostileBaseActionEventArgs> OnStartAiming;


    public override string GetName() => "Arrow Shot";
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => true;

    private float _stateTimer;
    private State _state;
    public Unit TargetUnit { get; private set; }
    public GridPosition TargetGridPosition { get; private set; }
    private int _enhanсedModifier = 1;
    private float _coolOffStateTime = 0.5f;
    private float _aimingStateTime = 1.5f;
    private float _rotationSpeed = 10f;


    private enum State
    {
        Aiming,

        // Shooting,
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
                var moveDirection = (TargetUnit.WorldPosition - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, _rotationSpeed * Time.deltaTime);
                break;
            // case State.Shooting:
            //     PerformAction(TargetGridPosition);
            //     break;
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
                _state = State.CoolOff;
                // var shootingStateTime = 0.1f;
                // PerformAction(TargetGridPosition);
                OnShot?.Invoke(this,
                    new OnHostileBaseActionEventArgs()
                        { targetGridPosition = TargetGridPosition, actionOnCastFinished = PerformAction });
                _stateTimer = _coolOffStateTime;
                break;
            // case State.Shooting:
            //     _state = State.CoolOff;
            //     // var coolOffStateTime = 0.5f;
            //     _stateTimer = _coolOffStateTime;
            //     break;
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
        _stateTimer = _aimingStateTime;
        OnStartAiming?.Invoke(this,
            new OnHostileBaseActionEventArgs()
            {
                actionOnCastFinished = null, targetGridPosition = TargetGridPosition, targetUnit = TargetUnit,
                unit = Unit
            });
        StartAction(actionOnComplete);
    }


    private void Shoot()
    {
        TargetUnit.TakeDamage(GetFinalDamage(Unit.Attack, TargetUnit.Defense));
        // OnShoot?.Invoke(this, new OnHostileBaseActionEventArgs { targetUnit = TargetUnit, unit = Unit });
        // OnAnyShoot?.Invoke(this, new OnHostileBaseActionEventArgs { targetUnit = TargetUnit, unit = Unit });
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
    protected override float GetModifier() => _enhanсedModifier;

    public void EnhanceAttack()
    {
        _enhanсedModifier = 3;
    }

    public void DisEnhanceAttack()
    {
        _enhanсedModifier = 1;
    }
}