using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    private List<Buff> _buffList = new();
    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        if (e.isPlayerTurn == _unit.IsEnemy) return;
        foreach (var buff in _buffList)
        {
            buff.ReduceCooldown();
            if (buff.CurrentCooldown <= 0) DeactivateBuff(buff);
        }
    }

    public void AddBuff(Buff buff)
    {
        ActivateBuff(buff);
        _buffList.Add(buff);
    }

    private void ActivateBuff(Buff buff)
    {
        _unit.ChangeCharacteristic(buff.CharacteristicType, buff.Value);
    }

    private void DeactivateBuff(Buff buff)
    {
        _unit.ChangeCharacteristic(buff.CharacteristicType, -buff.Value);
    }
}