using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public GridPosition GridPosition { get; private set; }
    public GridObject GridObject { get; private set; }
    public int GCost{ get; private set; }
    public int HCost{ get; private set; }
    public int FCost{ get; private set; }   
    public PathNode CameFromPathNode { get; private set; }
    public bool IsWalkable { get; private set; } = true;

    public PathNode(GridPosition gridPosition)
    {
        GridObject = LevelGrid.Instance.GridSystem.GetGridObject(GridPosition);
        GridPosition = gridPosition;
    }

    public override string ToString()
    {
        return GridPosition.ToString();
    }

    public void SetGCost(int cost)
    {
        GCost = cost;
        UpdateFCost();
    }
    public void UpdateFCost()
    {
        FCost = GCost + HCost;
    }
    public void SetHCost(int cost)
    {
        HCost = cost;
        UpdateFCost();
    }

    public void ResetCameFromNode()
    {
        CameFromPathNode = null;
    }

    public void SetCameFromPathNode(PathNode pathNode)
    {
        CameFromPathNode = pathNode;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
    }
    
}
