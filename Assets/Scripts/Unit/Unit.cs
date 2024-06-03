using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{
    public GridPosition GridPosition { get; private set; }
    public Vector3 WorldPosition => transform.position;
    public BaseAction[] ActionArray { get; private set; }
    [SerializeField] private bool _isEnemy;
    public bool IsEnemy => _isEnemy;
    public int MaxActionPoints => _unitCharacteristic.MaxActionPoints;
    public int MaxMovementPoints => _unitCharacteristic.Speed;

    public int ActionPoints { get; private set; }
    public int MovementPoints { get; private set; }
    public List<Buff> BuffList => _buffSystem.BuffList;

    // public int ActionPoints { get; private set; }
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler<UnitGridPositionEventArgs> OnAnyUnitDied;
    public static event EventHandler OnAnyUnitSpawned;
    public int Attack => _unitCharacteristic.Attack;
    public int Defense => _unitCharacteristic.Defense;
    public int MagicAttack => _unitCharacteristic.MagicAttack;
    public int Speed => _unitCharacteristic.Speed;
    public int Health => _healthSystem.Health;
    public int MaxHealth => _healthSystem.MaxHealth;

    public class UnitGridPositionEventArgs : EventArgs
    {
        public GridPosition gridPosition;
    }

    private HealthSystem _healthSystem;

    private UnitCharacteristic _unitCharacteristic;

    private BuffSystem _buffSystem;
    // Update is called once per frame

    private void Awake()
    {
        ActionArray = GetComponents<BaseAction>();
        _healthSystem = GetComponent<HealthSystem>();
        _unitCharacteristic = GetComponent<UnitCharacteristic>();
        _buffSystem = GetComponent<BuffSystem>();
    }

    private void Start()
    {
        ResetPoints();
        GridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(GridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        _healthSystem.OnDeath += HealthSystem_OnDeath;
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        Die();
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        if ((IsEnemy && !e.isPlayerTurn) || (!IsEnemy && e.isPlayerTurn))
        {
            ResetPoints();
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
        return action.GetActionPointsCost() <= ActionPoints;
    }

    private void SpendActionPoints(int amount)
    {
        ActionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction action)
    {
        if (!CanSpendActionPointsToTakeAction(action)) return false;
        SpendActionPoints(action.GetActionPointsCost());
        return true;
    }

    public bool CanSpendMovementPoints(int count)
    {
        return MovementPoints <= count;
    }

    public void SpendMovementPoints(int count)
    {
        MovementPoints -= count;
    }

    private void ResetPoints()
    {
        ActionPoints = MaxActionPoints;
        MovementPoints = MaxMovementPoints;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TakeDamage(int damage)
    {
        _healthSystem.TakeDamage(damage);
    }

    public void RestoreHealth(int count)
    {
        _healthSystem.RestoreHealth(count);
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

    public void ChangeCharacteristic(CharacteristicType characteristicType, int amount)
    {
        _unitCharacteristic.ChangeCharacteristic(characteristicType, amount);
    }

    public void AddBuff(Buff buff)
    {
        _buffSystem.AddBuff(buff);
    }
}