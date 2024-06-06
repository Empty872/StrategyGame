using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    private GridPosition _targetGridPosition;
    public static event EventHandler OnAnyExplosion;
    public event EventHandler OnExplosion;

    [SerializeField] private Transform _bulletHitVfxPrefab;

    // [SerializeField] private Transform _trail;
    [SerializeField] private AnimationCurve _arcYAnimationCurve;
    private float _totalDistance;

    private Vector3 _positionXZ;

    // private List<GridPosition> _affectedGridPositions;
    private Action<GridPosition> _onHitAffectAction;
    private float _timerToMove = 1;
    private bool _inArms = true;


    private void Update()
    {
        transform.rotation = Quaternion.identity;
        if (_timerToMove > 0)
        {
            _timerToMove -= Time.deltaTime;
            return;
        }

        if (_inArms)
        {
            transform.parent = null;
            _inArms = false;
        }

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
            // _trail.parent = null;
            Instantiate(_bulletHitVfxPrefab, _targetPosition + Vector3.up * 1, Quaternion.identity);
            _onHitAffectAction.Invoke(_targetGridPosition);
            Destroy(gameObject);
        }
    }


    public void Setup(GridPosition targetGridPosition,
        Action<GridPosition> onHitAffectAction)
    {
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        _targetGridPosition = targetGridPosition;
        _positionXZ = transform.position;
        _positionXZ.y = 0;
        _totalDistance = Vector3.Distance(_positionXZ, _targetPosition);
        _onHitAffectAction = onHitAffectAction;
    }
}