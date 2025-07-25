using UnityEngine;
using System;
using System.Collections.Generic;


public abstract class BaseAction : MonoBehaviour //No instance ever, so abstract. 

{
    /*  clearBusyOnActionComplete is a delegate, just a container, used to call ClearBusy from UnitActionSystem
        could ask for return or argument, etc. 
        action and func are 2 premade delegates. action is the same as void
    */
    protected Action clearBusyOnActionComplete;
    protected PCMech pCMech; //Protoected: can be accessed but not changed
    protected bool isActive;
    protected bool isPlayerAction = true;
    protected bool isEnemyAction = true;

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;



    protected virtual void Awake()
    {
        pCMech = GetComponent<PCMech>();
    }



    //==================================================================================================================================== 
    #region THE PROTECTED

    protected void ActionStart(Action clearBusyOnActionComplete)
    {
        isActive = true;
        this.clearBusyOnActionComplete = clearBusyOnActionComplete;
        pCMech.GainHeat(GetHeatGenerated());

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }
    protected void ActionComplete()
    {
        isActive = false;
        clearBusyOnActionComplete();

        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }
    // protected int GetRolledDamage(int bonusModifier = 0)
    // {
    //     var (tier1, tier2, tier3) = GetDamageByTier();
    //     PowerRoll.PowerRollTier tier = PowerRoll.Instance.Roll(bonusModifier);
    //     return tier switch
    //     {
    //         PowerRoll.PowerRollTier.Tier1 => tier1,
    //         PowerRoll.PowerRollTier.Tier2 => tier2,
    //         PowerRoll.PowerRollTier.Tier3 => tier3,
    //         _ => tier2,
    //     };
    // }
    protected (int damage, int knockback, PowerRoll.PowerRollTier tier) GetRolledDamageAndKnockback(int bonusModifier = 0)
    {
        var (d1, d2, d3) = GetDamageByTier();
        var (k1, k2, k3) = GetKnockbackByTier();

        PowerRoll.PowerRollTier tier = PowerRoll.Instance.Roll(bonusModifier);

        int damage = tier switch
        {
            PowerRoll.PowerRollTier.Tier1 => d1,
            PowerRoll.PowerRollTier.Tier2 => d2,
            PowerRoll.PowerRollTier.Tier3 => d3,
            _ => d2
        };

        int knockback = tier switch
        {
            PowerRoll.PowerRollTier.Tier1 => k1,
            PowerRoll.PowerRollTier.Tier2 => k2,
            PowerRoll.PowerRollTier.Tier3 => k3,
            _ => k2
        };

        return (damage, knockback, tier);
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
    public bool GetIsPlayerAction() => isPlayerAction; // Not used yet since all actions are player actions for now.
    public bool GetIsEnemyAction() => isEnemyAction;
    protected virtual (int tier1, int tier2, int tier3) GetDamageByTier() => (0, 0, 0);
    protected virtual (int tier1, int tier2, int tier3) GetKnockbackByTier() => (0, 0, 0);

    #endregion



    //==================================================================================================================================== 
    #region THE ABSTRACT

    public abstract string GetActionName();
    public abstract void TakeAction(GridPosition targetGridPosition, Action clearBusyOnActionComplete); //grid position useless for some, but oh well.
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
