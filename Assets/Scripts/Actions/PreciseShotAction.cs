using UnityEngine;

public class PreciseShotAction : ArrowShotAction
{
    public override string GetName() => "Precise Shot";
    public override string GetDescription() => "Attack enemy from distance using ATK. 50% to deal double damage";

    protected override float GetModifier() => Random.Range(1, 3);
    public override int GetCooldown() => 2;
    protected override int GetActionRange() => 4;
}