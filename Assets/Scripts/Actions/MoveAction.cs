using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    private int _maxMovementDistance = 4;
    private List<Vector3> _targetPositionList;
    private int _currentPositionIndex;
    private float _speed = 5;
    private float _stoppingDistance = 0.1f;
    private float _rotationSpeed = 10f;

    protected override void Awake()
    {
        base.Awake();
    }


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

    public override void TakeAction(GridPosition targetGridPosition, Action actionOnComplete)
    {
        var pathGridPositionList =
            Pathfinding.Instance.FindPath(Unit.GridPosition, targetGridPosition, out int pathLength);
        _currentPositionIndex = 0;
        _targetPositionList = new List<Vector3>();
        StartAction(actionOnComplete);
        foreach (var pathGridPosition in pathGridPositionList)
        {
            _targetPositionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
    }

    public override List<GridPosition> GetReachableActionGridPositionList()
    {
        var reachableGridPositionList = new List<GridPosition>();
        var unitGridPosition = Unit.GridPosition;

        for (int x = -_maxMovementDistance; x <= _maxMovementDistance; x++)
        {
            for (int z = -_maxMovementDistance; z <= _maxMovementDistance; z++)
            {
                var offsetGridPosition = new GridPosition(x, z);
                var possibleGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(possibleGridPosition))
                {
                    continue;
                }


                var possibleDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (possibleDistance > _maxMovementDistance) continue;
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
                    _maxMovementDistance * pathfindDistanceMultiplier) continue;
                reachableGridPositionList.Add(possibleGridPosition);
            }
        }

        return reachableGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        var validGridPositionList = new List<GridPosition>();
        foreach (var gridPosition in GetReachableActionGridPositionList())
        {
            validGridPositionList.Add(gridPosition);
        }

        return validGridPositionList;
    }

    public override string GetName() => "Move";

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = Unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction { gridPosition = gridPosition, actionPriority = targetCountAtGridPosition * 10 };
    }
}