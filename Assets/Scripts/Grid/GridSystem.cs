using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<TGridObject>
{
    public int Height { get; private set; }
    public int Width { get; private set; }
    public float CellSize { get; private set; }
    private TGridObject[,] _gridObjectsArray;

    public GridSystem(int height, int width, float cellSize,
        Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        Height = height;
        Width = width;
        CellSize = cellSize;
        _gridObjectsArray = new TGridObject[width, height];
        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                _gridObjectsArray[x, z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * CellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(Mathf.RoundToInt(worldPosition.x / CellSize),
            Mathf.RoundToInt(worldPosition.z / CellSize));
    }
    

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return _gridObjectsArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < Height && gridPosition.z < Width;
    }
}