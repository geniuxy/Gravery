using UnityEngine;

public class DeathBringerTriggers : EnemyAnimationTriggers
{
    private Boss_DeathBringer boss => GetComponentInParent<Boss_DeathBringer>();

    private void Relocate() => boss.FindTeleportPosition();

    private void MakeInvisible() => boss.fx.MakeTransparent(true);

    private void MakeVisible() => boss.fx.MakeTransparent(false);
}