using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class MechAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform bulletProjectilePrefab;
    [SerializeField] Transform shootPointTransform;
    [SerializeField] float shootDelay = 0.05f;


    void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<AntiMaterielAction>(out AntiMaterielAction antiMaterielAction))
        {
            antiMaterielAction.OnShoot += AntiMaterielAction_OnShoot;
            antiMaterielAction.OnShootCompleted += AntiMaterielAction_OnShootCompleted;
        }

        if (TryGetComponent<LaserMinigunAction>(out LaserMinigunAction laserMinigunAction))
        {
            laserMinigunAction.OnShoot += LaserMinigunAction_OnShoot;
        }

        if (TryGetComponent<MeleeAction>(out MeleeAction meleeAction))
        {
            meleeAction.OnMeleeActionStarted += MeleeAction_OnMeleeActionStarted;
            // meleeAction.OnMeleeActionCompleted += MeleeAction_OnMeleeActionCompleted;
        }
    }


    private void MeleeAction_OnMeleeActionStarted(object sender, EventArgs e)
    {
        animator.SetTrigger("MeleeSlash");
    }
    /* private void MeleeAction_OnMeleeActionCompleted(object sender, EventArgs e)
    {
    } */
    void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }

    void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void AntiMaterielAction_OnShoot(object sender, AntiMaterielAction.OnShootEventArgs e)
    {
        // animator.SetTrigger("Shoot");
        animator.SetBool("Shooting", true);

        /* StartCoroutine(ShootProjectileDelayed(e.targetMech)); */

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetMech.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y; //bullet fires horisontally
        bulletProjectile.Setup(targetUnitShootAtPosition);
    }
    private void AntiMaterielAction_OnShootCompleted(object sender, EventArgs e)
    {
        animator.SetBool("Shooting", false);
    }
    private void LaserMinigunAction_OnShoot(object sender, LaserMinigunAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        // StartCoroutine(ShootProjectileDelayed(e.targetMech));

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetUnitShootAtPosition = e.targetMech.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y; //bullet fires horisontally
        bulletProjectile.Setup(targetUnitShootAtPosition);
    }
    /* private IEnumerator ShootProjectileDelayed(PCMech targetMech)
    {
        yield return new WaitForSeconds(shootDelay);

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetPosition = targetMech.GetWorldPosition();
        targetPosition.y = shootPointTransform.position.y;
        bulletProjectile.Setup(targetPosition);
    } */
}
