using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private Unit _unit;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        TurnOff();
        if (UnitActionSystem.Instance.SelectedUnit == _unit) TurnOn();
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        TurnOff();
        if (UnitActionSystem.Instance.SelectedUnit == _unit) TurnOn();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        TurnOff();
        if (UnitActionSystem.Instance.SelectedUnit == _unit) TurnOn();
    }

    private void TurnOn()
    {
        gameObject.SetActive(true);
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}