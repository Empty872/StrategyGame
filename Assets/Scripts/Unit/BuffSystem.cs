using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    public List<Buff> BuffList { get; private set; } = new();
    private Unit _unit;
    public static event EventHandler OnAnyBuffListChanged;

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

        for (int i = 0; i < BuffList.Count; i++)
        {
            var buff = BuffList[i];
            buff.ReduceCooldown();
            if (buff.CurrentCooldown <= 0) RemoveBuff(buff);
        }
    }

    public void AddBuff(Buff buff)
    {
        ActivateBuff(buff);
        BuffList.Add(buff);
        OnAnyBuffListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveBuff(Buff buff)
    {
        DeactivateBuff(buff);
        BuffList.Remove(buff);
        OnAnyBuffListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ActivateBuff(Buff buff)
    {
        if (buff.CharacteristicType is not CharacteristicType.Null)
            _unit.ChangeCharacteristic(buff.CharacteristicType, buff.Value);
        buff.OnObtainAction?.Invoke();
    }

    private void DeactivateBuff(Buff buff)
    {
        if (buff.CharacteristicType is not CharacteristicType.Null)
            _unit.ChangeCharacteristic(buff.CharacteristicType, -buff.Value);
        buff.OnRemoveAction?.Invoke();
    }
}