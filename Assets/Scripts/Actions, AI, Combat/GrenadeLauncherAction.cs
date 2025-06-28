using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherAction : BaseAction
{
    [SerializeField] Transform grenadeProjectilePrefab;
    [SerializeField] float damageRadius = 8f; //ADDED

    int maxGrenadeDistance = 7;
    int pendingDamage;//ADDED



    protected override void Awake()
    {
        base.Awake();
        isEnemyAction = false; // Player only
    }
    void Update()
    {
        if (!isActive) { return; }
    }


    
    public override string GetActionName() => "Grenade Launcher";
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition pcMechGridPosition = pCMech.GetGridPosition();
        for (int x = -maxGrenadeDistance; x <= maxGrenadeDistance; x++)
        {
            for (int z = -maxGrenadeDistance; z <= maxGrenadeDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;// Skip invalid grid positions

                int testDistance = Math.Abs(x) + Math.Abs(z);
                if (testDistance > maxGrenadeDistance) continue; // Skip positions that are too far away

                validGridPositionList.Add(testGridPosition);// Add valid grid position to the list
            }
        }
        return validGridPositionList;
    }
    protected override (int, int, int) GetDamageByTier() => (8, 12, 18);
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        pendingDamage = GetRolledDamage();
        // int rolledDamage = GetRolledDamage();

        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, pCMech.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeExploded);

        Debug.Log("Grenade Launched");
        ActionStart(onActionComplete);
    }

    private void OnGrenadeExploded(Vector3 explosionPosition)
    {
        Collider[] colliderArray = Physics.OverlapSphere(explosionPosition, damageRadius);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out PCMech pcMech))
            {
                pcMech.TakeDamage(pendingDamage);
            }
        }
        ActionComplete();
    }
}
