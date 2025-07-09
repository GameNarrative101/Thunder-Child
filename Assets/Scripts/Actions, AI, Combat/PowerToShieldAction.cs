using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerToShieldAction : BaseAction
{
    public override string GetActionName() => "All Power to Shields";
    public override int GetCorePowerCost() => 3;
    public override int GetHeatGenerated() => 3;
    [SerializeField] int healAmount = 10;


    protected override void Awake()
    {
        base.Awake();
        isEnemyAction = false;
    }



    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
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
