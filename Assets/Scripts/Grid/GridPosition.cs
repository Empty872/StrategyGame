using System;
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
}