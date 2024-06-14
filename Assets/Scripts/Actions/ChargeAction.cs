using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargeAction : BaseAction
{
    public override GridColorEnum GetColor() => GridColorEnum.Red;
    protected override int GetActionRange() => 3;
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => true;
    public override string GetDescription() => "Move Unit";
    public override int GetCooldown() => 3;
    protected override float GetModifier() => 1.2f;
    protected override ActionRangeType GetActionRangeType() => ActionRangeType.VerticalHorizontal;
    private GridPosition _targetGridPosition;
    private Vector3 _pathEndPosition;
    private float _speed = 15;
    private float _stoppingDistance = 0.1f;
    // private float _rotationSpeed = 10f;


    private void Update()
    {
        if (!IsActive) return;
        Move();
    }

    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        var unit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        unit.TakeDamage(GetFinalDamage(Unit.Attack, unit.Defense));
    }

    public override void TakeAction(GridPosition targetGridPosition, Action actionOnComplete)
    {
        _targetGridPosition = targetGridPosition;
        var pathEndGridPosition = GridPosition.GetPreTargetGridPosition(Unit.GridPosition, targetGridPosition);
        _pathEndPosition = LevelGrid.Instance.GetWorldPosition(pathEndGridPosition);
        StartAction(actionOnComplete);
    }

    public override string GetName() => "Charge";

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionPriority = 0 };
    }

    private void Move()
    {
        if (Vector3.Distance(transform.position, _pathEndPosition) > _stoppingDistance)
        {
            var moveDirection = (_pathEndPosition - transform.position).normalized;
            transform.forward = moveDirection;
            transform.position += moveDirection * _speed * Time.deltaTime;
        }
        else
        {
            transform.position = _pathEndPosition;
            PerformAction(_targetGridPosition);
            CompleteAction();
        }
    }
}