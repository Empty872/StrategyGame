using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public event EventHandler OnActionStarted;
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
        OnActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void CompleteAction()
    {
        IsActive = false;
        OnActionComplete();
        TryUnselectAction();
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
    public virtual int GetActionPointsCost() => 1;

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

    public abstract string GetDescription();
}