using UnityEngine;
using System;
using System.Collections.Generic;

/*
    The stuff shared by all actions go here
    also we don't want this to have an instance ever, so abstract. 
    stuff that need to be instanciated are extended FROM this. just like we never want mono itself to have an instance on an object!
*/
public abstract class BaseAction : MonoBehaviour
{
    //can be accessed but not changed
    protected PCMech pCMech;
    protected bool isActive;
    
    /*
        define a delegate 
        just a container, nothing in it necessarily, could ask for return or argument, etc. 
        these have to match the function that is placed in this box
        action and func are 2 premade delegates. action is the same as void
    */
    protected Action onActionComplete;

    //virtual means what accesses this can override it when using it, but bc it's protected, the base won't change
    protected virtual void Awake()
    {
        pCMech = GetComponent<PCMech>();
    }

    /*
        abstract because every extension of this class MUST have this function or they cannot work
        this is used to get the name of the action for the UI button
    */
    public abstract string GetActionName();

    /*
        this is the generic take action function that all actions extending BaseAction will have. 
        Each one will then override this and do its own thing in that function
        not all actions need the grid position parameter which is annoying but most do so be it!
    */
    public abstract void TakeAction (GridPosition gridPosition, Action onActionComplete);

   
    //used to be on the moveaction class, but it's better for all actions to check for this since most use grid positions
    public virtual bool IsValidActionGridPosition (GridPosition gridPosition) 
    {
        List <GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    //a list of valid grid positions for the action. each action extending baseaction overrides this
    public abstract List<GridPosition> GetValidActionGridPositionList();


    //make every action declare how much core power it costs. each action overrides. defaults to 1
    public virtual int GetCorePowerCost()
    {
        return 1;
    }



    /*
        alternative way to handle the generic take action function with different paratmeters:

        define a BaseParameters CLASS here, pass that into the take action function as its only parameter
        then have each action extend that class with one of their own
        e.g. MoveBaseParameters, SpinBaseParameters, etc. 
        then the take action function would take in a BaseParameters object, and each action would pass in their own takeaction override like so:
        SpinBaseParameters spinBaseParameters = (SpinBaseParameters)baseParameters; inside the take action function
    */

}
