using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int _maxInteractionDistance = 1;
    protected override int GetActionRange() => 1;
    protected override bool CanBeUsedOnAllies() => false;
    protected override bool CanBeUsedOnEnemies() => false;
    protected override bool CanBeUsedOnInteractableObjects() => true;
    protected override ActionRangeType GetActionRangeType() => ActionRangeType.Square;
    

    public override void TakeAction(GridPosition targetGridPosition, Action actionOnComplete)
    {
        StartAction(actionOnComplete);
        PerformAction(targetGridPosition);
        CompleteAction();
    }
    protected override void AffectGridPosition(GridPosition gridPosition)
    {
        var interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(CompleteAction);
    }

    public override string GetName() => "Interact";

    public override GridColorEnum GetColor() => GridColorEnum.Yellow;
    public override int GetActionPointsCost() => 0;
    public override string GetDescription() => "Interact with object";

}