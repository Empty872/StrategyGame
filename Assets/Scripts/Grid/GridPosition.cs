using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public bool Equals(GridPosition other) => this == other;

    public override bool Equals(object obj) => obj is GridPosition gridPosition && Equals(gridPosition);

    public override int GetHashCode() => HashCode.Combine(x, z);

    public override string ToString()
    {
        return "x: " + x + "; z: " + z;
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x & a.z == b.z;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }

    public static int GetDistance(GridPosition a, GridPosition b)
    {
        var gridPosition = a - b;
        return Math.Abs(gridPosition.x) + Math.Abs(gridPosition.z);
    }

    public static List<GridPosition> GetStraightPathBetween(GridPosition a, GridPosition b)
    {
        var straightPath = new List<GridPosition>();
        if (a.x == b.x)
        {
            for (int z = Math.Min(a.z, b.z) + 1; z < Math.Max(a.z, b.z); z++)
            {
                straightPath.Add(new GridPosition(a.x, z));
            }
        }
        else if (a.z == b.z)
        {
            for (int x = Math.Min(a.x, b.x) + 1; x < Math.Max(a.x, b.x); x++)
            {
                straightPath.Add(new GridPosition(x, a.z));
            }
        }

        return straightPath;
    }

    public static GridPosition GetPreTargetGridPosition(GridPosition a, GridPosition b)
    {
        var straightPath = new List<GridPosition>();
        if (a.x == b.x)
        {
            if (a.z > b.z) return new GridPosition(a.x, b.z + 1);
            return new GridPosition(a.x, b.z - 1);
        }

        if (a.z == b.z)
        {
            if (a.x > b.x) return new GridPosition(b.x + 1, a.z);
            return new GridPosition(b.x - 1, a.z);
        }

        return new GridPosition(-1, -1);
    }
}