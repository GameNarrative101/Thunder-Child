using System;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    void Start()
    {
        AntiMaterielAction.OnAnyShoot += AntiMaterielAction_OnAnyShoot;
        LaserMinigunAction.OnAnyShoot += LaserMinigunAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        MeleeAction.OnAnyMeleeActionHit += MeleeAction_OnAnyMeleeActionHit;
    }

    private void LaserMinigunAction_OnAnyShoot(object sender, LaserMinigunAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(1f);
    }

    private void MeleeAction_OnAnyMeleeActionHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(0.5f);
    }

    private void AntiMaterielAction_OnAnyShoot(object sender, AntiMaterielAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(1f);
    }
    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(3f);
    }
}
