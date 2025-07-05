using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    [SerializeField] int interactPowerCost = 0;
    [SerializeField] int interactHeatGenerated = 0;
    int maxInteractDistance = 1;



    void Update()
    {
        if (!isActive) { return; }
        ActionComplete();
    }



    public override string GetActionName() => "Interact";
    public override int GetCorePowerCost() => interactPowerCost;
    public override int GetHeatGenerated() => interactHeatGenerated;
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition pcMechGridPosition = pCMech.GetGridPosition();
        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;// Skip invalid grid positions

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                if (interactable == null) continue;// Skip grid positions without an interactable object

                validGridPositionList.Add(testGridPosition);// Add valid grid position to the list
            }
        }
        return validGridPositionList;
    }
    public override void TakeAction(GridPosition gridPosition, Action clearBusyOnActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(clearBusyOnActionComplete);
        ActionStart(clearBusyOnActionComplete);
    }
}
