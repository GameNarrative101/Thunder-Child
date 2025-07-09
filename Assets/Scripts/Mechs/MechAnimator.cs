using System;
using Unity.Mathematics;
using UnityEngine;

public class MechAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform bulletProjectilePrefab;
    [SerializeField] Transform shootPointTransform;


    void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<AntiMaterielAction>(out AntiMaterielAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent<MeleeAction>(out MeleeAction meleeAction))
        {
            meleeAction.OnMeleeActionStarted += MeleeAction_OnMeleeActionStarted;
            meleeAction.OnMeleeActionCompleted += MeleeAction_OnMeleeActionCompleted;
        }
    }



    private void MeleeAction_OnMeleeActionStarted(object sender, EventArgs e)
    {
        animator.SetTrigger("MeleeSlash");
    }
    private void MeleeAction_OnMeleeActionCompleted(object sender, EventArgs e)
    {
    }
    void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking",true);
    }

    void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking",false);
    }
  
    private void ShootAction_OnShoot(object sender, AntiMaterielAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletProjectileTransform = Instantiate (bulletProjectilePrefab, shootPointTransform.position, quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
        
        Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        targetUnitShootAtPosition.y = shootPointTransform.position.y; //bullet fires horisontally
        bulletProjectile.Setup(targetUnitShootAtPosition);
    }
}
