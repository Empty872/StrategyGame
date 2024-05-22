using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public event EventHandler<OnUnitMovedEventArgs> OnAnyUnitMovedGridPosition;

    public class OnUnitMovedEventArgs : EventArgs
    {
        public GridPosition fromGridPosition;
        public GridPosition toGridPosition;
    }

    public static LevelGrid Instance { get; private set; }
    [SerializeField] private Transform _gridDebugObjectTransform;
    private GridSystem<GridObject> _gridSystem;
    public GridSystem<GridObject>  GridSystem => _gridSystem;
    private int _width = 10;
    private int _height = 10;
    private float _cellSize = 2f;
    public int Height => _gridSystem.Height;
    public int Width => _gridSystem.Width;
    public float CellSize => _gridSystem.CellSize;
    [SerializeField] private LayerMask _obstacleLayerMask;

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize,
            (gridSystem, gridPosition) => new GridObject(gridSystem, gridPosition));
    }

    private void Start()
    {
        SetObstacles();
        Pathfinding.Instance.Setup(Width, Height, CellSize);
    }


    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void AddDestructibleAtGridPosition(GridPosition gridPosition, Destructible destructible)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        gridObject.AddDestructible(destructible);
    }

    public void RemoveDestructibleAtGridPosition(GridPosition gridPosition, Destructible destructible)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveDestructible(destructible);
    }

    public void UnitMovedGridPosition(GridPosition fromGridPosition, GridPosition toGridPosition, Unit unit)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this,
            new OnUnitMovedEventArgs() { fromGridPosition = fromGridPosition, toGridPosition = toGridPosition });
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) => _gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition);

    public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);

    public bool IsPossibleGridPosition(GridPosition gridPosition, BaseAction action)
    {
        if (!_gridSystem.IsValidGridPosition(gridPosition)) return false;
        foreach (var actionGridPosition in action.GetValidActionGridPositionList())
        {
            if (gridPosition == actionGridPosition) return true;
        }

        return false;
    }

    public bool IsReachablePosition(GridPosition gridPosition, BaseAction action)
    {
        if (!_gridSystem.IsValidGridPosition(gridPosition)) return false;
        foreach (var actionGridPosition in action.GetReachableActionGridPositionList())
        {
            if (gridPosition == actionGridPosition) return true;
        }

        return false;
    }

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public bool HasAnyDestructibleOnGridPosition(GridPosition gridPosition)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Destructible GetDestructibleAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = _gridSystem.GetGridObject(gridPosition);
        return gridObject.GetDestructible();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, Door door)
    {
        _gridSystem.GetGridObject(gridPosition).SetInteractable(door);
    }

    public bool HasInteractableAtGridPosition(GridPosition gridPosition)
    {
        return _gridSystem.GetGridObject(gridPosition).HasInteractable();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        return _gridSystem.GetGridObject(gridPosition).GetInteractable();
    }

    public void SetObstacles()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                var gridObject = _gridSystem.GetGridObject(gridPosition) as GridObject;
                var worldPosition = GetWorldPosition(gridPosition);
                var raycastOffsetDistance = 5f;
                if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up,
                        raycastOffsetDistance * 2, _obstacleLayerMask))
                {
                    gridObject.SetObstacle();
                }
            }
        }
    }
}