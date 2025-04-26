using UnityEngine;
using System;
using System.Collections.Generic;

public class SpinAction : BaseAction
{
    
    float totalSpinAmount;

    private void Update()
    {
        if (!isActive) { return; }

        SpinTheMech();
    }

    private void SpinTheMech()
    {
        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;

        if (totalSpinAmount >= 360f) 
        { 
            ActionComplete();
        }
    }

    //put the delegate (still empty container) in a function. This means when we call this function, we can also put whatever other function we like into the delegate box and it'll run.
    //note: no () after the function name because we're not calling the function itself, we are referencing it
    //see UnitActionSystem for how clearing isBusy works.
    public override void TakeAction (GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        totalSpinAmount = 0f;
    }

    public override string GetActionName()
    {
        return "Federation Spin 3000";
    }


    //only do this action exactly where the unit is standing. it's just spinning
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List <GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition pcMechGridPosition = pCMech.GetGridPosition();

        return new List<GridPosition>
        {
            pcMechGridPosition
        };
    }



    public override int GetCorePowerCost()
    {
        return 2;
    }
    public override int GetHeatGenerated()
    {
        return 6;
    }
}