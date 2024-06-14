using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [SerializeField] private Transform _arrowProjectilePrefab;
    [SerializeField] private Transform _fireballProjectilePrefab;
    [SerializeField] private Transform _iceBoltProjectilePrefab;

    [SerializeField] private Transform _shootPointTransform;

    [SerializeField] private GameObject _sword;
    [SerializeField] private GameObject _swordTrail;
    [SerializeField] private GameObject _bow;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private Transform _spellPoint;
    [SerializeField] private Transform _buffVfx;
    [SerializeField] private Transform _debuffVfx;
    [SerializeField] private Transform _healVfx;
    private Unit _unit;
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsAiming = Animator.StringToHash("IsAiming");
    private static readonly int CastFriendlySpellTrigger = Animator.StringToHash("CastFriendlySpell");
    private static readonly int CastAttackSpellTrigger = Animator.StringToHash("CastAttackSpell");
    private static readonly int MeleeAttackTrigger = Animator.StringToHash("MeleeAttack");


    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        var arrowShotActions = GetComponents<ArrowShotAction>();
        foreach (var arrowShotAction in arrowShotActions)
        {
            arrowShotAction.OnShot += ArrowShotAction_OnShot;
            arrowShotAction.OnStartAiming += ArrowShotAction_OnStartAiming;
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
        }

        _unit = GetComponent<Unit>();
        _unit.OnActionCompleted += Unit_OnActionCompleted;
        _unit.OnBuffObtained += Unit_OnBuffObtained;
        _unit.OnHealthRestored += Unit_OnHealthRestored;
    }

    private void Start()
    {
        EquipDefault();
        HideArrow();
        _swordTrail.SetActive(false);
    }

    private void ArrowShotAction_OnShot(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        _animator.SetBool(IsAiming, false);
        var arrowTransform = Instantiate(_arrowProjectilePrefab, _arrow.transform.position, _arrow.transform.rotation);
        HideArrow();
        var arrow = arrowTransform.GetComponent<ArrowProjectile>();
        SetupProjectile(arrow, e.targetGridPosition, e.actionOnCastFinished);
    }

    private void ArrowShotAction_OnStartAiming(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        EquipNothing();
        EquipBow();
        _animator.SetBool(IsAiming, true);
        ShowArrow();
    }

    private void Unit_OnActionCompleted(object sender, EventArgs e)
    {
        EquipDefault();
        _swordTrail.SetActive(false);
    }

    private void Unit_OnHealthRestored(object sender, EventArgs e)
    {
        Instantiate(_healVfx, transform.position, Quaternion.identity);
    }

    private void Unit_OnBuffObtained(object sender, Unit.BuffEventArgs e)
    {
        var buffValue = e.buff.Value;
        if (buffValue == 0) return;
        var vfx = buffValue > 0 ? _buffVfx : _debuffVfx;
        Instantiate(vfx, transform.position, vfx.rotation);
    }

    private void BaseAction_OnFriendlyActionStarted(object sender, BaseAction.OnFriendlyActionEventArgs e)
    {
        EquipNothing();
        var targetPosition = LevelGrid.Instance.GetWorldPosition(e.targetGridPosition);
        transform.LookAt(targetPosition);
        _animator.SetTrigger(CastFriendlySpellTrigger);
        // e.actionOnCastFinished?.Invoke(e.targetGridPosition);
    }

    private void IceBoltAction_OnThrow(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        CastAttackSpell(_iceBoltProjectilePrefab, e);
    }


    private void SwordAction_OnSwordActionStarted(object sender, EventArgs e)
    {
        _animator.SetTrigger(MeleeAttackTrigger);
        EquipNothing();
        EquipSword();
        _swordTrail.SetActive(true);
    }

    private void FireballAction_OnThrow(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        CastAttackSpell(_fireballProjectilePrefab, e);
    }

    private void CastAttackSpell(Transform projectilePrefab, BaseAction.OnHostileBaseActionEventArgs e)
    {
        EquipNothing();
        _animator.SetTrigger(CastAttackSpellTrigger);
        var targetPosition = LevelGrid.Instance.GetWorldPosition(e.targetGridPosition);
        transform.LookAt(targetPosition);
        var spellTransform = Instantiate(projectilePrefab, _spellPoint.position, _spellPoint.rotation);
        var spellProjectile = spellTransform.GetComponent<AttackSpellProjectile>();
        SetupProjectile(spellProjectile, e.targetGridPosition, e.actionOnCastFinished);
    }

    private void SetupProjectile(Projectile projectile, GridPosition targetGridPosition,
        Action<GridPosition> onCastFinished)
    {
        projectile.Setup(targetGridPosition, onCastFinished);
    }

    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        _animator.SetBool(IsWalking, true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        _animator.SetBool(IsWalking, false);
    }

    private void EquipBow()
    {
        _bow.SetActive(true);
    }

    private void EquipSword()
    {
        _sword.SetActive(true);
    }

    private void EquipNothing()
    {
        _sword.SetActive(false);
        _bow.SetActive(false);
    }

    private void ShowArrow()
    {
        _arrow.SetActive(true);
    }

    private void HideArrow()
    {
        _arrow.SetActive(false);
    }

    private void EquipDefault()
    {
        EquipNothing();
        switch (_unit.UnitClass)
        {
            case UnitClass.Warrior:
                EquipSword();
                break;
            case UnitClass.Archer:
                EquipBow();
                break;
            default:
                break;
        }
    }
}