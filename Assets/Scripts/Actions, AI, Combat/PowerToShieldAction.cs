using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerToShieldAction : BaseAction
{
    [SerializeField] int healAmount = 10;
    [SerializeField] int corePowerCost = 3;
    [SerializeField] int heatGenerated = 3;


    protected override void Awake()
    {
        base.Awake();
        isEnemyAction = false;
    }



    public override string GetActionName() => "All Power to Shields";
    public override int GetCorePowerCost() => corePowerCost;
    public override int GetHeatGenerated() => heatGenerated;
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        // List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();

        return new List<GridPosition> { pcMechGridPosition };
    }

    public override void TakeAction(GridPosition targetGridPosition, Action clearBusyOnActionComplete)
    {
        ActionStart(clearBusyOnActionComplete);
        ShieldUp();
    }

    private void ShieldUp()
    {
        HealthSystem healthSystem = pCMech.GetComponent<HealthSystem>();
        healthSystem.Heal(healAmount);

        ActionComplete();
    }
}
