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






/* 
                                                    SPINNIN THANGS
==================================================================================================================================== 
*/
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






/* 
                                                    OVERRIDES
==================================================================================================================================== 
*/
    public override int GetCorePowerCost() => 2;
    public override int GetHeatGenerated() => 6;
    public override string GetActionName() => "Federation Spin 3000";

    //only do this action exactly where the unit is standing. it's just spinning
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List <GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();

        return new List<GridPosition> {pcMechGridPosition};
    }
    
    //put the delegate (still empty container) in a function. This means when we call this function, we can also put whatever other function we like into the delegate box and it'll run.
    //note: no () after the function name because we're not calling the function itself, we are referencing it
    //see UnitActionSystem for how clearing isBusy works.
    public override void TakeAction (GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0f;
        
        ActionStart(onActionComplete);
    }
}