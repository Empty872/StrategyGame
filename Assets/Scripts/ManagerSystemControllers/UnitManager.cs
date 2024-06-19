using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }
    private List<Unit> _unitList = new();
    private List<Unit> _friendlyUnitList = new();
    private List<Unit> _enemyUnitList = new();
    public event EventHandler OnAllFriendlyUnitsDied;
    public event EventHandler OnAllEnemiesDied;
    public List<Unit> UnitList => _unitList;
    public List<Unit> FriendlyUnitList => _friendlyUnitList;
    public List<Unit> EnemyUnitList => _enemyUnitList;

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDied += Unit_OnAnyUnitDied;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        var unit = sender as Unit;
        _unitList.Add(unit);
        if (unit.IsEnemy) _enemyUnitList.Add(unit);
        else _friendlyUnitList.Add(unit);
    }

    private void Unit_OnAnyUnitDied(object sender, EventArgs e)
    {
        var unit = sender as Unit;
        _unitList.Remove(unit);
        if (unit.IsEnemy) _enemyUnitList.Remove(unit);
        else _friendlyUnitList.Remove(unit);
        if (EnemyUnitList.Count == 0) OnAllEnemiesDied?.Invoke(this, EventArgs.Empty);
        if (UnitList.Count == 0) OnAllFriendlyUnitsDied?.Invoke(this, EventArgs.Empty);
    }
}