using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    int maxInteractDistance = 1;
    [SerializeField] int interactPowerCost = 0;



    void Update()
    {
        if (!isActive) { return; }
        ActionComplete();
    }



    public override int GetCorePowerCost() => interactPowerCost;
    public override string GetActionName() => "Interact";
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        // EXCEPTIONS DEFINE WHAT CAN BE INTERACTED WITH
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();
        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;// Skip invalid grid positions

                PrometheusCore prometheusCore = LevelGrid.Instance.GetPrometheusCoreAtGridPosition(testGridPosition);
                if (prometheusCore == null) continue;// Skip grid positions without a Prometheus Core

                validGridPositionList.Add(testGridPosition);// Add valid grid position to the list
            }
        }
        return validGridPositionList;
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        PrometheusCore prometheusCore = LevelGrid.Instance.GetPrometheusCoreAtGridPosition(gridPosition);
        prometheusCore.Interact();
        ActionStart(onActionComplete);
    }
}
