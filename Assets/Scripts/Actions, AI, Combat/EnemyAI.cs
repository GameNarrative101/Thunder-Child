using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
                            //CURRENT LOGIC IS FOR ALL ENEMIES TO GO BETWEEN PLAYER TURNS.

    enum State {WaitingForEnemyTurn, TakingTurn, Busy}
    State state;
    float timer;



    void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }
    void Start()
    {
        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnChanged;
    }
    void Update()
    {
        bool flowControl = HandleEnemyTurn();
        if (!flowControl) {return;}
    }



    private bool HandleEnemyTurn()
    {
        if (TurnSystemScript.Instance.IsPlayerTurn())
        {
            return false;
        }
        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryActivateEnemyAI(SetStateTakingTurn)) { state = State.Busy; }
                    else { TurnSystemScript.Instance.NextTurn(); }
                }
                break;
            case State.Busy:
                break;
        }

        return true;
    }
    private void TurnSystemScript_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystemScript.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }
    void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
    bool TryActivateEnemyAI(Action onEnemyAIActionComplete)
    {
        foreach (PCMech enemyUnit in UnitManager.Instance.GetEnemyMechList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) {return true;}
        }
        return false;
    }
    
    /* INEFFITIENT CODE, SEE SIMPLIFIED VERSION
         bool TryTakeEnemyAIAction(PCMech enemyUnit, Action onEnemyAIActionComplete)
        {
            EnemyAIAction bestEnemyAIAction = null;
            BaseAction bestBaseAction = null;

            foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
            {
                if (!enemyUnit.CanSpendCorePowerForAction(baseAction)) continue;

                EnemyAIAction newEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (newEnemyAIAction == null) continue;

                // Update the best action if it's better than the current best
                if (bestEnemyAIAction == null || newEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = newEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }

            if (bestEnemyAIAction != null && enemyUnit.TrySpendCorePowerForAction(bestBaseAction))
            {
                bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
                return true;
            }

            return false;
        } 
    */
    bool TryTakeEnemyAIAction(PCMech enemyUnit, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendCorePowerForAction(baseAction)) continue;

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction newEnemyAIAction = baseAction.GetBestEnemyAIAction();
                
                if (newEnemyAIAction == null) continue;
                if (newEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = newEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }
        
        if (bestEnemyAIAction != null && enemyUnit.TrySpendCorePowerForAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else return false;
    }

}
