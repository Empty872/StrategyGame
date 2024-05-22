using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GridPosition GridPosition { get; private set; }
    [SerializeField] private Transform _destroyedPrefab;
    public static event EventHandler OnAnyDestructibleDestruct;
    private void Start()
    {
        GridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddDestructibleAtGridPosition(GridPosition, this);
    }

    public void Destruct()
    {
        LevelGrid.Instance.RemoveDestructibleAtGridPosition(GridPosition, this);
        var destroyedTransform = Instantiate(_destroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToChildren(destroyedTransform, 150, transform.position, 10f);
        Destroy(gameObject);
        OnAnyDestructibleDestruct?.Invoke(this, EventArgs.Empty);
    }
    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}