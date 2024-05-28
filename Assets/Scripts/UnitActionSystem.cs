using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UnitActionSystem : MonoBehaviour
{
    [SerializeField] private Unit _selectedUnit;
    public Unit SelectedUnit => _selectedUnit;
    [SerializeField] private LayerMask _unitLayerMask;
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    private bool _isBusy;
    public bool IsBusy => _isBusy;
    public BaseAction SelectedAction { get; private set; }
    public static UnitActionSystem Instance { get; private set; }

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
        SelectUnit(_selectedUnit);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        if (e.isPlayerTurn) SelectUnit(UnitManager.Instance.FriendlyUnitList[0]);
        else SelectUnit(UnitManager.Instance.EnemyUnitList[0], true);
    }

    void Update()
    {
        if (_isBusy) return;
        if (!TurnSystem.Instance.IsPlayerTurn) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (TryHandleUnitSelection()) return;
        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsLeftMouseButtonDownThisFrame())
        {
            if (SelectedAction is null) return;
            var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            if (!SelectedAction.IsValidActionGridPosition(mouseGridPosition)) return;
            if (!_selectedUnit.TrySpendActionPointsToTakeAction(SelectedAction)) return;
            SetBusy();
            SelectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool TryHandleUnitSelection()
    {
        if (!InputManager.Instance.IsRightMouseButtonDownThisFrame()) return false;
        var ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, _unitLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out Unit unit))
            {
                if (_selectedUnit == unit) return false;
                if (unit.IsEnemy) return false;
                SelectUnit(unit);
                return true;
            }
        }

        return false;
    }

    public void SelectUnit(Unit unit, bool isEnemy = false)
    {
        _selectedUnit = unit;
        if (!isEnemy) SelectAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetBusy()
    {
        _isBusy = true;
        OnBusyChanged?.Invoke(this, _isBusy);
    }

    private void ClearBusy()
    {
        _isBusy = false;
        OnBusyChanged?.Invoke(this, _isBusy);
    }

    public void SelectAction(BaseAction action)
    {
        SelectedAction = action;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
}