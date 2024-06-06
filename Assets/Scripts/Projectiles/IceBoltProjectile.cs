using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IceBoltProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    private GridPosition _targetGridPosition;

    [SerializeField] private Transform _iceBoltHitVfxPrefab;

    private Vector3 _positionXZ;

    // private List<GridPosition> _affectedGridPositions;
    private Action<GridPosition> _onHitAffectAction;
    private float _timerToMove = 1;
    private bool _inArms = true;
    private float _moveSpeed = 15f;
    private float _reachedTargetDistance = 0.2f;
    private float _targetPositionY = 1;


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

        Vector3 moveDir = (_targetPosition - transform.position).normalized;
        transform.position += moveDir * _moveSpeed * Time.deltaTime;
        transform.LookAt(_targetPosition);
        if (Vector3.Distance(transform.position, _targetPosition) < _reachedTargetDistance)
        {
            Instantiate(_iceBoltHitVfxPrefab, _targetPosition, Quaternion.identity);
            _onHitAffectAction.Invoke(_targetGridPosition);
            Destroy(gameObject);
        }
    }


    public void Setup(GridPosition targetGridPosition,
        Action<GridPosition> onHitAffectAction)
    {
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition) + Vector3.up * _targetPositionY;
        _targetGridPosition = targetGridPosition;
        _positionXZ = transform.position;
        _positionXZ.y = 0;
        _onHitAffectAction = onHitAffectAction;
    }
}