using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherAction : BaseAction
{
    [SerializeField] Transform grenadeProjectilePrefab;

    int maxGrenadeDistance = 7;



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
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, pCMech.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, onGrenadeBehaviourComplete);

        Debug.Log("Grenade Launched");
        ActionStart(onActionComplete);
    }
    void onGrenadeBehaviourComplete()
    {
        ActionComplete();
    }
}
