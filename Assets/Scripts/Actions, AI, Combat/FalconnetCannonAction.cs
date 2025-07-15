using System;
using System.Collections.Generic;
using UnityEngine;

public class FalconnetCannonAction : BaseAction
{
    [SerializeField] Transform grenadeProjectilePrefab;
    [SerializeField] float damageRadius = 8f; //ADDED
    [SerializeField] int maxGrenadeDistance = 7;



    protected override void Awake()
    {
        base.Awake();
        isEnemyAction = false; // Player only
    }



    public override string GetActionName() => "Falconnet Cannon";
    protected override (int, int, int) GetDamageByTier() => (3, 5, 8);
    protected override (int, int, int) GetKnockbackByTier() => (2, 4, 6);
    public override int GetCorePowerCost() => 3;
    public override int GetHeatGenerated() => 3;

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
