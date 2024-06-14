using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform _actionButtonContainer;
    [SerializeField] private Transform _actionButtonPrefab;
    [SerializeField] private TextMeshProUGUI _actionPointsText;

    private List<ActionButtonUI> _actionButtonUIList = new();

    // Start is called before the first frame update
    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
        // TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        CreateUnitActionButtons();
        UpdateVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        if (UnitActionSystem.Instance.SelectedUnit.IsEnemy)
        {
            ClearUnitActionButtons();
        }
        else
        {
            CreateUnitActionButtons();
        }

        UpdateVisual();
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void CreateUnitActionButtons()
    {
        ClearUnitActionButtons();

        var selectedUnit = UnitActionSystem.Instance.SelectedUnit;
        if (selectedUnit.IsEnemy) return;
        foreach (var action in selectedUnit.ActionArray)
        {
            var actionButtonTransform = Instantiate(_actionButtonPrefab, _actionButtonContainer);
            var actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetAction(action);
            _actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void ClearUnitActionButtons()
    {
        foreach (Transform actionButton in _actionButtonContainer)
        {
            Destroy(actionButton.gameObject);
        }

        _actionButtonUIList.Clear();
    }

    private void UpdateVisual()
    {
        foreach (var actionButtonUI in _actionButtonUIList)
        {
            actionButtonUI.UpdateVisual();
        }
    }

    private void UpdateActionPoints()
    {
        if (UnitActionSystem.Instance.SelectedUnit.IsEnemy) _actionPointsText.enabled = false;
        else
        {
            _actionPointsText.text = "Action Points: " + UnitActionSystem.Instance.SelectedUnit.ActionPoints;
            _actionPointsText.enabled = true;
        }
    }

    // private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    // {
    //     UpdateActionPoints();
    // }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
}