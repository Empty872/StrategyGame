using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;
    public Unit Unit { get; private set; }
    protected bool IsActive { get; private set; }

    protected Action OnActionComplete { get; private set; }

    protected virtual void Awake()
    {
        Unit = GetComponent<Unit>();
    }

    protected void StartAction(Action action)
    {
        IsActive = true;
        OnActionComplete = action;
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void CompleteAction()
    {
        IsActive = false;
        OnActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public abstract string GetName();

    public virtual List<GridPosition> GetAffectedGridPositionList(GridPosition targetGridPosition)
    {
        var affectedGridPositionList = new List<GridPosition>();
        if (IsValidActionGridPosition(targetGridPosition)) affectedGridPositionList.Add(targetGridPosition);
        return affectedGridPositionList;
    }

    public abstract void TakeAction(GridPosition gridPosition, Action actionOnComplete);

    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        var validActionGridPositionList = GetValidActionGridPositionList();
        return validActionGridPositionList.Contains(gridPosition);
    }


    public abstract List<GridPosition> GetReachableActionGridPositionList();
    public abstract List<GridPosition> GetValidActionGridPositionList();
    public virtual int GetCost() => 1;

    public EnemyAIAction GetBestEnemyAIAction()
    {
        var enemyAIActionList = new List<EnemyAIAction>();
        var validActionGridPositionList = GetValidActionGridPositionList();
        foreach (var gridPosition in validActionGridPositionList)
        {
            var enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count == 0) return null;
        enemyAIActionList = enemyAIActionList.OrderByDescending(element => element.actionPriority).ToList();
        return enemyAIActionList[0]; 
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}