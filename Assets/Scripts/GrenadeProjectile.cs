using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    public static event EventHandler OnAnyExplosion;
    public event EventHandler OnExplosion;
    [SerializeField] private Transform _bulletHitVfxPrefab;
    [SerializeField] private Transform _trail;
    [SerializeField] private AnimationCurve _arcYAnimationCurve;
    private float _totalDistance;
    private Vector3 _positionXZ;
    private List<GridPosition> _affectedGridPositions;
    private Action<List<GridPosition>> _onHitAffectAction;


    private void Update()
    {
        Vector3 moveDir = (_targetPosition - _positionXZ).normalized;

        var moveSpeed = 15f;
        _positionXZ += moveDir * moveSpeed * Time.deltaTime;
        var distance = Vector3.Distance(_positionXZ, _targetPosition);
        var distanceNormalized = 1 - distance / _totalDistance;
        var maxHeight = _totalDistance / 4f;
        var positionY = _arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(_positionXZ.x, positionY, _positionXZ.z);
        var reachedTargetDistance = 0.2f;
        if (Vector3.Distance(_positionXZ, _targetPosition) < reachedTargetDistance)
        {
            OnAnyExplosion?.Invoke(this, EventArgs.Empty);
            OnExplosion?.Invoke(this, EventArgs.Empty);
            _trail.parent = null;
            Instantiate(_bulletHitVfxPrefab, _targetPosition + Vector3.up * 1, Quaternion.identity);
            _onHitAffectAction.Invoke(_affectedGridPositions);
            Destroy(gameObject);
        }
    }


    public void Setup(GridPosition targetGridPosition, List<GridPosition> affectedGridPositions,
        Action<List<GridPosition>> onHitAffectAction)
    {
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        _positionXZ = transform.position;
        _positionXZ.y = 0;
        _totalDistance = Vector3.Distance(_positionXZ, _targetPosition);
        _affectedGridPositions = affectedGridPositions;
        _onHitAffectAction = onHitAffectAction;
    }
}