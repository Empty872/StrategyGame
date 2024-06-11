using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackSpellProjectile : Projectile
{
    private float _timerToMove = 1f;
    private bool _inArms = true;
    protected override float GetSpeed() => 15f;

    private void Start()
    {
        SetHitPoint(transform);
    }


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

        Move();
    }
}