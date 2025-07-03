using System;
using System.Collections.Generic;
using UnityEngine;

public class HeatSinkAction : BaseAction
{
    int heatSinkCharges = 3;



    public override string GetActionName() => "Heat Sink";
    public override int GetCorePowerCost() => 0;
    public override int GetHeatGenerated() => 0;
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();

        return new List<GridPosition> { pcMechGridPosition };
    }
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        if (!(heatSinkCharges == 0))
        {
            heatSinkCharges--;
            HeatSink();
            Debug.Log($"Heat Sink used. Remaining charges: {heatSinkCharges}");
        }
        else
        {
            Debug.Log("No heat sink charges left!");
            ActionComplete();
        }
    }
    private void HeatSink()
    {
        pCMech.ResetHeat();
        ActionComplete();
    }
    public int GetHeatSinkCharges()
    {
        return heatSinkCharges;
    }
}
