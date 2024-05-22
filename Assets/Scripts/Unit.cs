using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public GridPosition GridPosition { get; private set; }
    public Vector3 WorldPosition => transform.position;
    public BaseAction[] ActionArray { get; private set; }
    [SerializeField] private bool _isEnemy;
    public bool IsEnemy => _isEnemy;
    private int _maxActionPoints = 10;
    public int ActionPoints { get; private set; }
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler<UnitGridPositionEventArgs> OnAnyUnitDied;
    public static event EventHandler OnAnyUnitSpawned;

    public class UnitGridPositionEventArgs : EventArgs
    {
        public GridPosition gridPosition;
    }

    private HealthSystem _healthSystem;

    // Update is called once per frame

    private void Awake()
    {
        ActionArray = GetComponents<BaseAction>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        ResetActionPoints();
        GridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(GridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        _healthSystem.OnDeath += HealthSystemOnDeath;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void HealthSystemOnDeath(object sender, EventArgs e)
    {
        Die();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy && !TurnSystem.Instance.IsPlayerTurn) || (!IsEnemy && TurnSystem.Instance.IsPlayerTurn))
        {
            ResetActionPoints();
        }
    }

    void Update()
    {
        var newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != GridPosition)
        {
            var oldGridPosition = GridPosition;
            GridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGridPosition(oldGridPosition, newGridPosition, this);
        }
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction action)
    {
        return action.GetCost() <= ActionPoints;
    }

    private void SpendActionPoints(int amount)
    {
        ActionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction action)
    {
        if (!CanSpendActionPointsToTakeAction(action)) return false;
        SpendActionPoints(action.GetCost());
        return true;
    }

    private void ResetActionPoints()
    {
        ActionPoints = _maxActionPoints;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TakeDamage(float damage)
    {
        _healthSystem.TakeDamage(damage);
    }

    private void Die()
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(GridPosition, this);
        Destroy(gameObject);
        OnAnyUnitDied?.Invoke(this, new UnitGridPositionEventArgs { gridPosition = GridPosition });
    }

    public float GetHealthNormalized() => _healthSystem.GetHealthNormalized();

    public T GetAction<T>() where T : BaseAction
    {
        foreach (var action in ActionArray)
        {
            if (action is T rightAction) return rightAction;
        }

        return null;
    }
}