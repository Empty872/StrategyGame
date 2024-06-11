using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Projectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    private GridPosition _targetGridPosition;
    [SerializeField] private Transform _hitPoint;
    [SerializeField] private Transform _trail;
    [SerializeField] private Transform _hitVfxPrefab;

    private Action<GridPosition> _onHitAffectAction;

    // private float _speed = 25f;
    private float _reachedTargetDistance = 0.2f;
    protected virtual float GetSpeed() => 25f;
    private float _targetPositionY = 1.3f;

    public void Setup(GridPosition targetGridPosition,
        Action<GridPosition> onHitAffectAction)
    {
        _targetGridPosition = targetGridPosition;
        _targetPosition = LevelGrid.Instance.GetWorldPosition(_targetGridPosition) + Vector3.up * _targetPositionY;
        transform.LookAt(_targetPosition);
        _onHitAffectAction = onHitAffectAction;
    }

    protected void Move()
    {
        var moveDir = (_targetPosition - transform.position).normalized;
        transform.position += moveDir * GetSpeed() * Time.deltaTime;
        if (Vector3.Distance(_targetPosition, _hitPoint.position) < _reachedTargetDistance)
        {
            if (_trail != null)
            {
                Debug.Log(_trail);
                _trail.parent = null;
            };
            Instantiate(_hitVfxPrefab, _targetPosition, Quaternion.identity);
            _onHitAffectAction.Invoke(_targetGridPosition);
            Destroy(gameObject);
        }
    }

    protected void SetHitPoint(Transform hitPoint)
    {
        _hitPoint = hitPoint;
    }
}