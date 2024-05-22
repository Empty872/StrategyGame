using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float _rotationSpeed = 360;
    private float _currentRotatedAngle;

    void Update()
    {
        if (!IsActive) return;
        transform.eulerAngles += new Vector3(0, _rotationSpeed * Time.deltaTime, 0);
        _currentRotatedAngle += _rotationSpeed * Time.deltaTime;
        if (_currentRotatedAngle >= 360) CompleteAction();
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        _currentRotatedAngle = 0;
        StartAction(actionOnComplete);
    }

    public override string GetName() => "Spin";


    public override List<GridPosition> GetReachableActionGridPositionList()
    {
        return new List<GridPosition> { Unit.GridPosition };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetReachableActionGridPositionList();
    }

    public override int GetCost() => 2;

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction { gridPosition = gridPosition, actionPriority = 0 };
    }
}