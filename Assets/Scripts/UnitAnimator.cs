using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletProjectilePrefab;
    [SerializeField] private Transform _grenadeProjectilePrefab;
    [SerializeField] private Transform _shootPointTransform;
    [SerializeField] private GameObject _rifle;
    [SerializeField] private GameObject _sword;


    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent(out GrenadeAction grenadeAction))
        {
            grenadeAction.OnThrow += GrenadeAction_OnThrow;
        }

        if (TryGetComponent(out SwordAction swordAction))
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
    }

    private void Start()
    {
        EquipRifle();
    }

    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        EquipSword();
        _animator.SetTrigger("SwordSlash");
    }

    private void SwordAction_OnSwordActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void GrenadeAction_OnThrow(object sender, GrenadeAction.OnThrowEventArgs e)
    {
        var grenadeTransform = Instantiate(_grenadeProjectilePrefab, transform.position, Quaternion.identity);
        var grenade = grenadeTransform.GetComponent<GrenadeProjectile>();
        var targetUnitGridPosition = e.targetGridPosition;
        grenade.Setup(targetUnitGridPosition, 
            e.onHitAffectAction);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        _animator.SetTrigger("Shoot");
        var bulletTransform = Instantiate(_bulletProjectilePrefab, _shootPointTransform.position, Quaternion.identity);
        var bullet = bulletTransform.GetComponent<BulletProjectile>();
        var targetUnitPosition = e.targetUnit.WorldPosition;
        targetUnitPosition.y = _shootPointTransform.position.y;
        bullet.Setup(targetUnitPosition);
    }

    private void EquipRifle()
    {
        _sword.SetActive(false);
        _rifle.SetActive(true);
    }

    private void EquipSword()
    {
        _rifle.SetActive(false);
        _sword.SetActive(true);
    }
}