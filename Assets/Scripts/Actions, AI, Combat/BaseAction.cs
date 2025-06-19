using UnityEngine;
using System;
using System.Collections.Generic;


public abstract class BaseAction : MonoBehaviour //No instance ever, so abstract. 

{
    /*  onActionComplete is a delegate, just a container, nothing in it necessarily, could ask for return or argument, etc. 
        action and func are 2 premade delegates. action is the same as void*/
    protected Action onActionComplete;
    protected PCMech pCMech; //Protoected: can be accessed but not changed
    protected bool isActive;
    [SerializeField] protected bool isPlayerAction = true;
    [SerializeField] protected bool isEnemyAction = true;

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;



    protected virtual void Awake()
    {
        pCMech = GetComponent<PCMech>();
    }



    //==================================================================================================================================== 
    #region THE PROTECTED

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
    #region THE VIRTUAL

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }
    public virtual int GetCorePowerCost() => 1;
    public virtual int GetHeatGenerated() => 1;
    public bool IsPlayerAction() => isPlayerAction; // Not used yet since all actions are player actions for now.
    public bool IsEnemyAction() => isEnemyAction;

    #endregion



    //==================================================================================================================================== 
    #region THE ABSTRACT

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
            if (enemyAIAction == null)
            {
                Debug.LogWarning($"{GetType().Name}: GetBestEnemyAIAction returned null for {gridPosition}");
            }
            else
            {
                enemyAIActionList.Add(enemyAIAction);
            }
        }

        if (enemyAIActionList.Count == 0) { return null; }

        enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
        return enemyAIActionList[0];
    }

    #endregion
}
