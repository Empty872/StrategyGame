using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _bulletProjectilePrefab;
    [SerializeField] private Transform _fireballProjectilePrefab;
    [SerializeField] private Transform _iceBoltProjectilePrefab;
    [SerializeField] private Transform _shootPointTransform;
    [SerializeField] private GameObject _rifle;
    [SerializeField] private GameObject _sword;
    [SerializeField] private Transform _spellPoint;
    public const float FriendlySpellCastAnimationTime = 3f;


    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        var shootActions = GetComponents<ShootAction>();
        foreach (var shootAction in shootActions)
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent(out FireballAction fireballAction))
        {
            fireballAction.OnThrow += FireballAction_OnThrow;
        }

        if (TryGetComponent(out IceBoltAction iceBoltAction))
        {
            iceBoltAction.OnThrow += IceBoltAction_OnThrow;
        }

        var baseActions = GetComponents<BaseAction>();
        foreach (var baseAction in baseActions)
        {
            baseAction.OnFriendlyActionStarted += BaseAction_OnFriendlyActionStarted;
        }


        var swordActions = GetComponents<SwordAction>();
        foreach (var swordAction in swordActions)
        {
            swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
            // swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        }
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        // if (TryGetComponent(out SwordAction swordAction))
        // {
        //     swordAction.OnSwordActionStarted += SwordAction_OnSwordActionStarted;
        //     swordAction.OnSwordActionCompleted += SwordAction_OnSwordActionCompleted;
        // }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void BaseAction_OnFriendlyActionStarted(object sender, BaseAction.OnFriendlyActionEventArgs e)
    {
        EquipNothing();
        var targetPosition = LevelGrid.Instance.GetWorldPosition(e.targetGridPosition);
        transform.LookAt(targetPosition);
        _animator.SetTrigger("CastFriendlySpell");
        e.actionOnCastFinished?.Invoke(e.targetGridPosition);
    }

    private void IceBoltAction_OnThrow(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        EquipNothing();
        var targetPosition = LevelGrid.Instance.GetWorldPosition(e.targetGridPosition);
        transform.LookAt(targetPosition);
        _animator.SetTrigger("CastAttackSpell");
        var iceBoltTransform = Instantiate(_iceBoltProjectilePrefab, _spellPoint.position, _spellPoint.rotation);
        iceBoltTransform.parent = _spellPoint;
        var iceBolt = iceBoltTransform.GetComponent<IceBoltProjectile>();
        var targetUnitGridPosition = e.targetGridPosition;
        iceBolt.Setup(targetUnitGridPosition,
            e.actionOnCastFinished);
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

    private void FireballAction_OnThrow(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        EquipNothing();
        _animator.SetTrigger("CastAttackSpell");
        var fireballTransform = Instantiate(_fireballProjectilePrefab, _spellPoint.position, _spellPoint.rotation);
        fireballTransform.parent = _spellPoint;
        var fireball = fireballTransform.GetComponent<FireballProjectile>();
        var targetUnitGridPosition = e.targetGridPosition;
        fireball.Setup(targetUnitGridPosition,
            e.actionOnCastFinished);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnHostileBaseActionEventArgs e)
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

    private void EquipNothing()
    {
        _sword.SetActive(false);
        _rifle.SetActive(false);
    }
}