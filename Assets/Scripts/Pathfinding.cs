using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [SerializeField] private Transform gridDebugObjectPrefab;
    private int _height = 10;
    private int _width = 10;
    private float _cellSize = 2;
    private GridSystem<PathNode> _gridSystem;
    private int _moveStraightCost = 10;
    private int _moveDiagonalCost = 14;
    public static Pathfinding Instance { get; private set; }
    [SerializeField] private LayerMask _obstacleLayerMask;

    public void Setup(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _gridSystem =
            new GridSystem<PathNode>(_width, _height, _cellSize,
                (gridSystem, gridPosition) => new PathNode(gridPosition));
        // _gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        UpdateIsWalkable();
    }

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
        Unit.OnAnyUnitDied += Unit_OnAnyUnitDied;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        Destructible.OnAnyDestructibleDestruct += Destructible_OnAnyDestructibleDestruct;
    }

    private void Destructible_OnAnyDestructibleDestruct(object sender, EventArgs e)
    {
        var destructible = sender as Destructible;
        SetIsWalkableGridPosition(destructible.GridPosition, true);
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnUnitMovedEventArgs e)
    {
        SetIsWalkableGridPosition(e.fromGridPosition, true);
        SetIsWalkableGridPosition(e.toGridPosition, false);
    }

    private void Unit_OnAnyUnitDied(object sender, Unit.UnitGridPositionEventArgs e)
    {
        SetIsWalkableGridPosition(e.gridPosition, true);
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        var openList = new List<PathNode>();
        var closedList = new List<PathNode>();
        var startNode = _gridSystem.GetGridObject(startGridPosition);
        var endNode = _gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);
        for (int x = 0; x < _gridSystem.Width; x++)
        {
            for (int z = 0; z < _gridSystem.Height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                var pathNode = _gridSystem.GetGridObject(gridPosition);
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.ResetCameFromNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        while (openList.Count > 0)
        {
            var currentNode = GetLowestFCostPathNode(openList);
            if (currentNode == endNode)
            {
                pathLength = endNode.FCost;
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);
            foreach (var neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.IsWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                var tentativeGCost = currentNode.GCost +
                                     CalculateDistance(currentNode.GridPosition, neighbourNode.GridPosition);
                if (tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GridPosition, endGridPosition));
                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        var gridPositionDistance = gridPositionA - gridPositionB;
        var xDistance = Math.Abs(gridPositionDistance.x);
        var zDistance = Math.Abs(gridPositionDistance.z);
        var remainingStraightDistance = Math.Abs(xDistance - zDistance);
        var distance = Math.Min(xDistance, zDistance) * _moveDiagonalCost +
                       remainingStraightDistance * _moveStraightCost;
        return distance;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        var lowestFCostPathNode = pathNodeList[0];
        for (var i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].FCost < lowestFCostPathNode.FCost)
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        var neighbourList = new List<PathNode>();
        var gridPosition = currentNode.GridPosition;
        var leftNodeIsGood = gridPosition.x > 0 && GetNode(gridPosition.x - 1, gridPosition.z + 0).IsWalkable;
        var rightNodeIsGood = gridPosition.x < _gridSystem.Width - 1 &&
                              GetNode(gridPosition.x + 1, gridPosition.z + 0).IsWalkable;
        var upNodeIsGood = gridPosition.z < _gridSystem.Height - 1 &&
                           GetNode(gridPosition.x + 0, gridPosition.z + 1).IsWalkable;
        var downNodeIsGood = gridPosition.z > 0 && GetNode(gridPosition.x + 0, gridPosition.z - 1).IsWalkable;
        if (leftNodeIsGood) neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 0));
        if (rightNodeIsGood) neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 0));
        if (upNodeIsGood) neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z + 1));
        if (downNodeIsGood) neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.z - 1));
        if (leftNodeIsGood && upNodeIsGood) neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
        if (leftNodeIsGood && downNodeIsGood) neighbourList.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
        if (rightNodeIsGood && upNodeIsGood) neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));
        if (rightNodeIsGood && downNodeIsGood) neighbourList.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
        return neighbourList;
    }

    private PathNode GetNode(int x, int z)
    {
        return _gridSystem.GetGridObject(new GridPosition(x, z));
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        var pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        var currentNode = endNode;
        while (currentNode.CameFromPathNode != null)
        {
            pathNodeList.Add(currentNode.CameFromPathNode);
            currentNode = currentNode.CameFromPathNode;
        }

        pathNodeList.Reverse();

        return pathNodeList.Select(pathNode => pathNode.GridPosition).ToList();
    }

    private void UpdateIsWalkable()
    {
        for (var x = 0; x < _width; x++)
        {
            for (var z = 0; z < _height; z++)
            {
                var gridPosition = new GridPosition(x, z);
                var gridObject = LevelGrid.Instance.GridSystem.GetGridObject(gridPosition);
                
                SetIsWalkableGridPosition(gridPosition, !gridObject.HasObstacle);
            }
        }
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        _gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return _gridSystem.GetGridObject(gridPosition).IsWalkable;
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        var hasPath = FindPath(startGridPosition, endGridPosition, out int pathLength);
        return hasPath is not null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}