using UnityEngine;
using System;
using System.Collections.Generic;

//The stuff shared by all actions go here
//Monobehaviour here means all other actions get theirs' extended from Base action instead of mono, and can still be mono
//also we don't want this to have an instance ever, so abstract. stuff that need to be instanciated are extended FROM this. just like we never want mono itself to have an instance on an object!
public abstract class BaseAction : MonoBehaviour
{
    //can be accessed but not changed
    protected PCMech pCMech;
    protected bool isActive;
    //define a delegate (just a container, nothing in it necessarily, could ask for return or argument, etc. these have to match the function that is placed in this box)
    //action and func are 2 premade delegates. action is the same as void
    protected Action onActionComplete;



    //virtual means what accesses this can override it when using it, but bc it's protected, the base won't change
    protected virtual void Awake()
    {
        pCMech = GetComponent<PCMech>();
    }



    //abstract because every extension of this class MUST have this function or they cannot work
    public abstract string GetActionName();

    //this is the generic take action function that all actions extending BaseAction will have. Each one will then override this and do its own thing in that function
    //not all actions need the grid position parameter which is annoying but so be it!
    public abstract void TakeAction (GridPosition gridPosition, Action onActionComplete);

   
    //used to be on the moveaction class, but it's better for all actions to check for this
    public virtual bool IsValidActionGridPosition (GridPosition gridPosition) 
    {
        List <GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    //a list of valid grid positions for the action. each action extending baseaction overrides this
    public abstract List<GridPosition> GetValidActionGridPositionList();

    //all actions cost 1 core power, but this can be overridden in actions with different costs
    public virtual int GetCorePowerCost()
    {
        return 1;
    }
}
