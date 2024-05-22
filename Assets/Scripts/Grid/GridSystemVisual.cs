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
    [SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList;

    private Dictionary<GridVisualType, Material> _gridVisualTypeMaterialDictionary = new();
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
        foreach (var gridVisualTypeMaterial in _gridVisualTypeMaterialList)
        {
            var gridVisualType = gridVisualTypeMaterial.GridVisualType;
            var material = gridVisualTypeMaterial.Material;
            _gridVisualTypeMaterialDictionary.Add(gridVisualType, material);
        }

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

    public void ShowReachableGridPositions(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        foreach (var gridPosition in gridPositions)
        {
            _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z]
                .Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void ShowGridPosition(GridPosition gridPosition, GridVisualType gridVisualType)
    {
        _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
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
        var selectedAction = UnitActionSystem.Instance.SelectedAction;
        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default:
            case MoveAction:
                gridVisualType = GridVisualType.BlueSoft;
                break;
            case ShootAction:
                gridVisualType = GridVisualType.RedSoft;
                break;
            case SpinAction:
                gridVisualType = GridVisualType.BlueSoft;
                break;
            case GrenadeAction:
                gridVisualType = GridVisualType.RedSoft;
                break;
            case SwordAction :
                gridVisualType = GridVisualType.RedSoft;
                break;
            case InteractAction :
                gridVisualType = GridVisualType.GreenSoft;
                break;
        }

        ShowReachableGridPositions(selectedAction.GetReachableActionGridPositionList(), gridVisualType);
    }

    public void UpdateMouseGridVisual()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        var mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
        var selectedAction = UnitActionSystem.Instance.SelectedAction;
        List<GridPosition> mouseGridPositionList = new List<GridPosition>();
        if (LevelGrid.Instance.IsValidGridPosition(mouseGridPosition))
            mouseGridPositionList = selectedAction.GetAffectedGridPositionList(mouseGridPosition);
        GridVisualType highlightGridVisualType;
        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default:
            case MoveAction:
                highlightGridVisualType = GridVisualType.Blue;
                gridVisualType = GridVisualType.BlueSoft;
                break;
            case ShootAction:
                highlightGridVisualType = GridVisualType.Red;
                gridVisualType = GridVisualType.RedSoft;
                break;
            case SpinAction:
                highlightGridVisualType = GridVisualType.Blue;
                gridVisualType = GridVisualType.BlueSoft;
                break;
            case GrenadeAction:
                highlightGridVisualType = GridVisualType.Red;
                gridVisualType = GridVisualType.RedSoft;
                break;
            case SwordAction:
                highlightGridVisualType = GridVisualType.Red;
                gridVisualType = GridVisualType.RedSoft;
                break;
            case InteractAction :
                highlightGridVisualType = GridVisualType.Green;
                gridVisualType = GridVisualType.GreenSoft;
                break;
        }

        foreach (var gridPosition in _mouseGridPositionList)
        {
            HideGridPosition(gridPosition);
            if (LevelGrid.Instance.IsReachablePosition(gridPosition, selectedAction))
            {
                ShowGridPosition(gridPosition, gridVisualType);
            }

            _mouseGridPositionList = mouseGridPositionList;
        }

        _mouseGridPositionList = mouseGridPositionList;
        foreach (var gridPosition in _mouseGridPositionList)
        {
            ShowGridPosition(gridPosition, highlightGridVisualType);
        }


        // Old
        // if (LevelGrid.Instance.IsValidGridPosition(_mouseGridPositionList))
        // {
        //     HideGridPosition(_mouseGridPositionList);
        // }
        //
        // if (LevelGrid.Instance.IsReachablePosition(_mouseGridPositionList, selectedAction))
        // {
        //     ShowGridPosition(_mouseGridPositionList, gridVisualType);
        // }
        //
        // _mouseGridPositionList = mouseGridPositionList;
        //
        // if (LevelGrid.Instance.IsPossibleGridPosition(_mouseGridPositionList, selectedAction))
        //     ShowGridPosition(_mouseGridPositionList, highlightGridVisualType);
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType) =>
        _gridVisualTypeMaterialDictionary[gridVisualType];
}