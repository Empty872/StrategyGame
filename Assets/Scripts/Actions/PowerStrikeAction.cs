using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerStrikeAction : SwordAction
{
    public override string GetName() => "Power Strike";
    protected override float GetModifier() => 1.5f;
    public override int GetCooldown() => 2;
    public override string GetDescription() => "Attack enemy in melee combat using 1.5 * ATK";
}