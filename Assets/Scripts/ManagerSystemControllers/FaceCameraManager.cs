using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCameraManager : MonoBehaviour
{
    private Unit _currentUnit;

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        _currentUnit = UnitActionSystem.Instance.SelectedUnit;
        SwitchCamera(UnitActionSystem.Instance.SelectedUnit);
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        SwitchCamera(UnitActionSystem.Instance.SelectedUnit);
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        SwitchCamera(UnitActionSystem.Instance.SelectedUnit);
    }

    private void TurnOn()
    {
        _currentUnit.FaceCamera.SetActive(true);
    }

    private void TurnOff()
    {
        _currentUnit.FaceCamera.SetActive(false);
    }

    private void SwitchCamera(Unit unit)
    {
        TurnOff();
        _currentUnit = unit;
        TurnOn();
    }
}