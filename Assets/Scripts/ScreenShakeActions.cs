using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        // FireballProjectile.OnAnyExplosion += GrenadeProjectile_OnAnyExplosion;
        SwordAction.OnAnyAttack += SwordAction_OnAnyAttack;
    }

    private void SwordAction_OnAnyAttack(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake();
    }

    // private void GrenadeProjectile_OnAnyExplosion(object sender, EventArgs e)
    // {
    //     ScreenShake.Instance.Shake(5);
    // }

    private void ShootAction_OnAnyShoot(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
}
