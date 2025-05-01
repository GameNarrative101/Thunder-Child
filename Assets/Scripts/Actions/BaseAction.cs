using UnityEngine;
using System;
using System.Collections.Generic;


public abstract class BaseAction : MonoBehaviour //No instance ever, so abstract. 

{
    /*
        define a delegate 
        just a container, nothing in it necessarily, could ask for return or argument, etc. 
        these have to match the function that is placed in this box
        action and func are 2 premade delegates. action is the same as void
    */
    protected Action onActionComplete;
    protected PCMech pCMech; //Protoected: can be accessed but not changed
    protected bool isActive;
    protected bool isEnemyAction = false;

    public static event EventHandler OnAnyActionStarted; //NOT USED YET
    public static event EventHandler OnAnyActionCompleted; //NOT USED YET
    





    /* 
        virtual means what accesses this can override it when using it, 
        but bc it's protected, we can call it from actions that extend this and the base won't change 
    */
    protected virtual void Awake()
    {
        pCMech = GetComponent<PCMech>();
        if (pCMech == null)
        {
            Debug.LogError($"PCMech component not found on {gameObject.name}");
        }
    }






/* 
                                                    THE PROTECTED
==================================================================================================================================== 
*/
    protected void ActionStart (Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        pCMech.GainHeat(GetHeatGenerated());

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }






/* 
                                            THE VIRTUAL (defaults if not overridden)
==================================================================================================================================== 
*/   
    public virtual bool IsValidActionGridPosition (GridPosition gridPosition) 
    {
        List <GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }
    public virtual int GetCorePowerCost() => 1;
    public virtual int GetHeatGenerated() => 1;
    public virtual bool GetIsEnemyAction => isEnemyAction;






/* 
                                        THE ABSTRACT (every extension MUST have these)
==================================================================================================================================== 
*/ 
    public abstract string GetActionName();
    public abstract void TakeAction (GridPosition gridPosition, Action onActionComplete); //useless for some, but oh well.
    public abstract List<GridPosition> GetValidActionGridPositionList();









    /*
        alternative way to handle the generic take action function with different paratmeters:

        define a BaseParameters CLASS here, pass that into the take action function as its only parameter
        then have each action extend that class with one of their own
        e.g. MoveBaseParameters, SpinBaseParameters, etc. 
        then the take action function would take in a BaseParameters object, and each action would pass in their own takeaction override like so:
        SpinBaseParameters spinBaseParameters = (SpinBaseParameters)baseParameters; inside the take action function
    */

}
