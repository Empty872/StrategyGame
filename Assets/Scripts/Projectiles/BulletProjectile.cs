using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    [SerializeField] private Transform _trail;
    [SerializeField] private Transform _bulletHitVfxPrefab;


    // Update is called once per frame
    void Update()
    {
        var moveDir = (_targetPosition - transform.position).normalized;
        var speed = 200f;
        var beforeDistance = Vector3.Distance(_targetPosition, transform.position);
        transform.position += moveDir * speed * Time.deltaTime;
        var afterDistance = Vector3.Distance(_targetPosition, transform.position);
        if (beforeDistance < afterDistance)
        {
            transform.position = _targetPosition;
            _trail.parent = null;
            Instantiate(_bulletHitVfxPrefab, _targetPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void Setup(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }
}