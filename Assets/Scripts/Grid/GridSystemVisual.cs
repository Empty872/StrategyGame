using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform _gridSystemVisualSinglePrefab;
    private GridSystemVisualSingle[,] _gridSystemVisualSingleArray;
    public static GridSystemVisual Instance { get; private set; }
    private List<GridPosition> _mouseGridPositionList = new();

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
        _gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.Width, LevelGrid.Instance.Height];
        for (var x = 0; x < LevelGrid.Instance.Width; x++)
        {
            for (var z = 0; z < LevelGrid.Instance.Height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                _gridSystemVisualSingleArray[x, z] = Instantiate(_gridSystemVisualSinglePrefab,
                    LevelGrid.Instance.GetWorldPosition(gridPosition),
                    Quaternion.identity).GetComponent<GridSystemVisualSingle>();
            }
        }

        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateGridVisual();
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool e)
    {
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateMouseGridVisual();
    }

    public void ShowReachableGridPositions(List<GridPosition> gridPositions)
    {
        foreach (var gridPosition in gridPositions)
        {
            ShowReachableGridPosition(gridPosition);
        }
    }

    public void ShowReachableGridPosition(GridPosition gridPosition)
    {
        _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z]
            .Show(GetReachableGridMaterial());
    }

    public void ShowPossibleGridPositions(List<GridPosition> gridPositions)
    {
        foreach (var gridPosition in gridPositions)
        {
            ShowPossibleGridPosition(gridPosition);
        }
    }

    public void ShowPossibleGridPosition(GridPosition gridPosition)
    {
        _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z]
            .Show(GetPossibleGridMaterial());
    }

    public void HideAllGridPositions()
    {
        for (var x = 0; x < LevelGrid.Instance.Width; x++)
        {
            for (var z = 0; z < LevelGrid.Instance.Height; z++)
            {
                _gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void HideGridPosition(GridPosition gridPosition)
    {
        _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Hide();
    }

    public void UpdateGridVisual()
    {
        HideAllGridPositions();
        if (UnitActionSystem.Instance.IsBusy) return;
        if (!TurnSystem.Instance.IsPlayerTurn) return;
        var selectedAction = UnitActionSystem.Instance.SelectedAction;
        if (selectedAction is not null) ShowReachableGridPositions(selectedAction.GetReachableActionGridPositionList());
    }

    public void UpdateMouseGridVisual()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (UnitActionSystem.Instance.IsBusy) return;
        if (!TurnSystem.Instance.IsPlayerTurn) return;
        var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        var selectedAction = UnitActionSystem.Instance.SelectedAction;
        var mouseGridPositionList = new List<GridPosition>();
        if (LevelGrid.Instance.IsValidGridPosition(mouseGridPosition) && selectedAction is not null)
            mouseGridPositionList = selectedAction.GetAffectedGridPositionList(mouseGridPosition);

        foreach (var gridPosition in _mouseGridPositionList)
        {
            HideGridPosition(gridPosition);
            if (LevelGrid.Instance.IsReachablePosition(gridPosition, selectedAction))
            {
                ShowReachableGridPosition(gridPosition);
            }

            _mouseGridPositionList = mouseGridPositionList;
        }

        _mouseGridPositionList = mouseGridPositionList;
        foreach (var gridPosition in _mouseGridPositionList)
        {
            ShowPossibleGridPosition(gridPosition);
        }
    }

    private Material GetReachableGridMaterial()
    {
        var action = UnitActionSystem.Instance.SelectedAction;
        return GridVisualManager.Instance.GetReachableColor(action);
    }

    private Material GetPossibleGridMaterial()
    {
        var action = UnitActionSystem.Instance.SelectedAction;
        return GridVisualManager.Instance.GetPossibleColor(action);
    }
}