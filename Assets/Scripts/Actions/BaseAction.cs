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

    public static event EventHandler OnAnyActionStarted; //NOT USED YET
    public static event EventHandler OnAnyActionCompleted; //NOT USED YET



    /* 
        virtual means what accesses this can override it when using it, 
        but bc it's protected, we can call it from actions that extend this and the base won't change 
    */
    protected virtual void Awake()
    {
        pCMech = GetComponent<PCMech>();
    }



    //==================================================================================================================================== 
    #region PROTECTED
    protected void ActionStart(Action onActionComplete)
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
    #endregion



    //==================================================================================================================================== 
    #region VIRTUAL
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }
    public virtual int GetCorePowerCost() => 1;
    public virtual int GetHeatGenerated() => 1;
    #endregion



    //==================================================================================================================================== 
    #region ABSTRACT
    public abstract string GetActionName();
    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete); //useless for some, but oh well.
    public abstract List<GridPosition> GetValidActionGridPositionList();
    #endregion



    //==================================================================================================================================== 
    #region ENEMY AI
    public virtual EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition) { return null; }
    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetBestEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count == 0) { return null; }

        enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
        return enemyAIActionList[0];
    }
    #endregion
}
