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
        if (_currentRotatedAngle >= 360) PerformAction(Unit.GridPosition);
    }

    public override void TakeAction(GridPosition gridPosition, Action actionOnComplete)
    {
        _currentRotatedAngle = 0;
        StartAction(actionOnComplete);
    }
    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        return;
    }

    public override string GetName() => "Spin";
    public override GridColorEnum GetColor() => GridColorEnum.Green;

    protected override int GetActionRange() => 0;
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => false;
    public override string GetDescription() => "Spin";

    public override int GetActionPointsCost() => 2;
}