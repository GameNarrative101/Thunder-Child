using System;
using System.Collections.Generic;
using UnityEngine;

public class FalconnetCannonAction : BaseAction
{
    [SerializeField] int maxGrenadeDistance = 7;
    [SerializeField] Vector3Int knockbackByTier = new Vector3Int(2, 4, 6);
    [SerializeField] Vector3Int enemyDamageByTier = new Vector3Int(3, 5, 8);
    [SerializeField] Vector3Int playerDamageByTier = new Vector3Int(9, 15, 24);
    [SerializeField] int heatGenerated = 3;
    [SerializeField] int corePowerCost = 3;
    [SerializeField] float damageRadius = 8f; //ADDED
    [SerializeField] Transform grenadeProjectilePrefab;



    protected override void Awake()
    {
        base.Awake();
        // isEnemyAction = false; // Player only
    }



    public override string GetActionName() => "Falconnet Cannon";
    protected override (int, int, int) GetDamageByTier()
    {
       if (!pCMech.GetIsEnemy())
       {
           return (playerDamageByTier.x, playerDamageByTier.y, playerDamageByTier.z);
       }
       else
       {
           return (enemyDamageByTier.x, enemyDamageByTier.y, enemyDamageByTier.z);
       }
    }
    protected override (int, int, int) GetKnockbackByTier() => (knockbackByTier.x, knockbackByTier.y, knockbackByTier.z);
    public override int GetCorePowerCost() => corePowerCost;
    public override int GetHeatGenerated() => heatGenerated;

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(pCMech.GetGridPosition());
    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition simulatedPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxGrenadeDistance; x <= maxGrenadeDistance; x++)
        {
            for (int z = -maxGrenadeDistance; z <= maxGrenadeDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = simulatedPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;

                int testDistance = Math.Abs(x) + Math.Abs(z);
                if (testDistance > maxGrenadeDistance) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }
    public override void TakeAction(GridPosition targetGridPosition, Action clearBusyOnActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, pCMech.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(targetGridPosition, OnGrenadeExploded);

        if (pCMech.GetIsEnemy()) EnemyActionTaken = true;

        ActionStart(clearBusyOnActionComplete);
    }

    private void OnGrenadeExploded(Vector3 explosionPosition)
    {
        (int damage, int knockback, PowerRoll.PowerRollTier tier) = GetRolledDamageAndKnockback();

        Collider[] colliderArray = Physics.OverlapSphere(explosionPosition, damageRadius);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out PCMech pcMech))
            {
                pcMech.TakeDamage(damage);
                pcMech.Knockback(knockback, LevelGrid.Instance.GetGridPosition(explosionPosition));
            }
        }
        ActionComplete();
    }
    

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition simulatedPosition)
    {
        List<GridPosition> targetList = GetValidActionGridPositionList(simulatedPosition);
        if (targetList.Count == 0) return null;

        return new EnemyAIAction
        {
            gridPosition = simulatedPosition,
            actionValue = 80,
        };
    }
}
