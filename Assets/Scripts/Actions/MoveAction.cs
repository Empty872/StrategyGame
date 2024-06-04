using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public override GridColorEnum GetColor() => GridColorEnum.Blue;

    protected override int GetActionRange() => Unit.MovementPoints;
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => false;
    protected override bool CanBeUsedOnEmpty() => true;
    public override string GetDescription() => "Move Unit";
    public override int GetActionPointsCost() => 0;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    private List<Vector3> _targetPositionList;
    private int _currentPositionIndex;
    private float _speed = 5;
    private float _stoppingDistance = 0.1f;
    private float _rotationSpeed = 10f;


    private void Update()
    {
        if (!IsActive) return;
        var targetPosition = _targetPositionList[_currentPositionIndex];
        var moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, _rotationSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) > _stoppingDistance)
        {
            transform.position += moveDirection * _speed * Time.deltaTime;
            OnStartMoving?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            _currentPositionIndex++;
            if (_currentPositionIndex >= _targetPositionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                CompleteAction();
            }
        }
    }

    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        return;
    }

    public override List<GridPosition> GetReachableActionGridPositionList()
    {
        var reachableGridPositionList = new List<GridPosition>();
        var unitGridPosition = Unit.GridPosition;

        for (int x = -GetActionRange(); x <= GetActionRange(); x++)
        {
            for (int z = -GetActionRange(); z <= GetActionRange(); z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var possibleGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(possibleGridPosition))
                {
                    continue;
                }


                var possibleDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (possibleDistance > GetActionRange()) continue;
                if (Unit.GridPosition == possibleGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(possibleGridPosition))
                {
                    // Grid Position already occupied with another Unit
                    continue;
                }

                // if (!Pathfinding.Instance.IsWalkableGridPosition(possibleGridPosition)) continue;
                if (!Pathfinding.Instance.HasPath(unitGridPosition, possibleGridPosition)) continue;
                var pathfindDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(unitGridPosition, possibleGridPosition) >
                    GetActionRange() * pathfindDistanceMultiplier) continue;
                reachableGridPositionList.Add(possibleGridPosition);
            }
        }

        return reachableGridPositionList;
    }

    public override void TakeAction(GridPosition targetGridPosition, Action actionOnComplete)
    {
        var pathGridPositionList =
            Pathfinding.Instance.FindPath(Unit.GridPosition, targetGridPosition, out int pathLength);
        _currentPositionIndex = 0;
        _targetPositionList = new List<Vector3>();
        foreach (var pathGridPosition in pathGridPositionList)
        {
            _targetPositionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        StartAction(actionOnComplete);
        Unit.SpendMovementPoints(GridPosition.GetDistance(targetGridPosition, Unit.GridPosition));
    }

    public override string GetName() => "Move";

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionPriority = 10 };
    }
}