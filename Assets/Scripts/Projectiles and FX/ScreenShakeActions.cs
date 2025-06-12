using System;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    void Start()
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        MeleeAction.OnAnyMeleeActionHit += MeleeAction_OnAnyMeleeActionHit;
    }



    private void MeleeAction_OnAnyMeleeActionHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(0.5f);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake(1f);
    }
    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(3f);
    }
}
