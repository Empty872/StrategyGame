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
    private int _cooldown;
    private int _currentCooldown;
    public int CurrentCooldown => _currentCooldown;

    protected virtual void Awake()
    {
        Unit = GetComponent<Unit>();
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        if (e.isPlayerTurn) ReduceCurrentCooldown();
    }

    protected void StartAction(Action action)
    {
        if (!CanBeUsed()) return;
        IsActive = true;
        OnActionComplete = action;
        ActivateCooldown();
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void CompleteAction()
    {
        IsActive = false;
        OnActionComplete.Invoke();
        TryUnselectAction();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public abstract string GetName();

    public virtual List<GridPosition> GetAffectedGridPositionList(GridPosition targetGridPosition)
    {
        var affectedGridPositionList = new List<GridPosition>();
        if (!LevelGrid.Instance.IsPossibleGridPosition(targetGridPosition, this)) return affectedGridPositionList;
        for (var x = -GetTargetRange(); x < GetTargetRange() + 1; x++)
        {
            for (var z = -GetTargetRange(); z < GetTargetRange() + 1; z++)
            {
                var gridPosition = targetGridPosition + new GridPosition(x, z);

                if (LevelGrid.Instance.IsValidGridPosition(gridPosition))
                    affectedGridPositionList.Add(gridPosition);
            }
        }

        return affectedGridPositionList;
    }


    protected virtual void PerformAction(GridPosition targetGridPosition)
    {
        var gridPositionList = GetAffectedGridPositionList(targetGridPosition);
        foreach (var gridPosition in gridPositionList)
        {
            AffectGridPosition(gridPosition);
        }
    }

    protected abstract void AffectGridPosition(GridPosition gridPosition);


    public abstract void TakeAction(GridPosition gridPosition, Action actionOnComplete);

    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        var validActionGridPositionList = GetPossibleActionGridPositionList();
        return validActionGridPositionList.Contains(gridPosition);
    }


    // public abstract List<GridPosition> GetReachableActionGridPositionList();
    public virtual List<GridPosition> GetReachableActionGridPositionList()
    {
        var unitGridPosition = Unit.GridPosition;
        var reachableGridPositionList = new List<GridPosition>();
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

                if (possibleGridPosition == Unit.GridPosition && !CanBeUsedOnOneself()) continue;

                var possibleDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (GetActionRangeType() != ActionRangeType.Square && possibleDistance > GetActionRange()) continue;
                if (GetActionRangeType() == ActionRangeType.VerticalHorizontal && x != 0 && z != 0) continue;
                reachableGridPositionList.Add(possibleGridPosition);
            }
        }

        return reachableGridPositionList;
    }

    // public abstract List<GridPosition> GetPossibleActionGridPositionList();
    public virtual List<GridPosition> GetPossibleActionGridPositionList()
    {
        var possibleGridPositionList = new List<GridPosition>();
        foreach (var gridPosition in GetReachableActionGridPositionList())
        {
            if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition) &&
                !LevelGrid.Instance.HasInteractableAtGridPosition(gridPosition) && !CanBeUsedOnEmpty())
            {
                // Grid Position is empty;
                continue;
            }

            var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

            // Both units in same team
            if (targetUnit is not null && targetUnit.IsEnemy == Unit.IsEnemy && !CanBeUsedOnAllies()) continue;

            // Both units in different teams
            if (targetUnit is not null && targetUnit.IsEnemy != Unit.IsEnemy && !CanBeUsedOnEnemies()) continue;
            if (LevelGrid.Instance.HasInteractableAtGridPosition(gridPosition) &&
                !CanBeUsedOnInteractableObjects()) continue;

            possibleGridPositionList.Add(gridPosition);
        }

        return possibleGridPositionList;
    }

    public virtual int GetActionPointsCost() => 1;

    public EnemyAIAction GetBestEnemyAIAction()
    {
        var enemyAIActionList = new List<EnemyAIAction>();
        var validActionGridPositionList = GetPossibleActionGridPositionList();
        foreach (var gridPosition in validActionGridPositionList)
        {
            var enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count == 0) return null;
        // Debug.Log(enemyAIActionList.Count);
        enemyAIActionList = enemyAIActionList.OrderByDescending(element => element.actionPriority).ToList();
        return enemyAIActionList[0];
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    private void ReduceCurrentCooldown()
    {
        _currentCooldown -= 1;
        _currentCooldown = Mathf.Max(_currentCooldown, 0);
    }

    private void ActivateCooldown()
    {
        _currentCooldown = GetCooldown();
    }

    public bool CanBeUsed() => _currentCooldown <= 0;

    public virtual int GetCooldown() => 0;
    public abstract GridColorEnum GetColor();
    protected virtual float GetModifier() => 1;

    protected int GetFinalDamage(int attack, int enemyDefense)
    {
        var baseDamage = attack - enemyDefense;
        var finalDamage = Mathf.Max((int)(baseDamage * GetModifier()), 1);
        return finalDamage;
    }

    private void TryUnselectAction()
    {
        if (!CanBeUsed()) UnitActionSystem.Instance.SelectAction(null);
    }

    protected abstract int GetActionRange();
    protected virtual int GetTargetRange() => 0;
    protected virtual ActionRangeType GetActionRangeType() => ActionRangeType.Rhombus;
    protected virtual bool CanBeUsedOnOneself() => false;
    protected abstract bool CanBeUsedOnAllies();
    protected abstract bool CanBeUsedOnEnemies();
    protected virtual bool CanBeUsedOnEmpty() => false;
    protected virtual bool CanBeUsedOnInteractableObjects() => false;

    public abstract string GetDescription();
}