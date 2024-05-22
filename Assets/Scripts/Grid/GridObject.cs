using System.Collections.Generic;
using System.Linq;

public class GridObject
{
    private GridSystem<GridObject> _gridSystem;
    private GridPosition _gridPosition;
    private List<Unit> _units = new();
    private List<Destructible> _destructibles = new();
    private IInteractable _interactable;
    public bool HasObstacle { get; private set; }

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        _gridSystem = gridSystem;
        _gridPosition = gridPosition;
    }

    public override string ToString()
    {
        var unitString = _units.Aggregate("", (current, unit) => current + (unit + "\n"));

        return _gridPosition + "\n" + unitString;
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }
    public void AddDestructible(Destructible destructible)
    {
        _destructibles.Add(destructible);
    }

    public void RemoveDestructible(Destructible destructible)
    {
        _destructibles.Remove(destructible);
    }

    public List<Unit> GetUnits() => _units;
    public bool HasAnyUnit() => _units.Count > 0;
    public bool HasAnyDestructible() => _destructibles.Count > 0;
    public List<Destructible> GetDestructibles() => _destructibles;

    public Unit GetUnit() => !HasAnyUnit() ? null : GetUnits()[0];
    public Destructible GetDestructible() => !HasAnyDestructible() ? null : GetDestructibles()[0];
    public IInteractable GetInteractable() => _interactable;

    public void SetInteractable(IInteractable interactable)
    {
        _interactable = interactable;
    }

    public bool HasInteractable() => _interactable is not null;

    public void SetObstacle()
    {
        HasObstacle = true;
    }
}