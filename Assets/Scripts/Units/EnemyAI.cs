using System;
using System.Threading;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    enum State { WaitingForTurn, TakingTurn, Busy}

    State state;
    float timer;






    private void Awake()
    {
        state = State.WaitingForTurn;
    }
    private void Start()
    {
        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
    }
    private void Update()
    {
        bool flowControl = TryHandleEnemyTurn();
        if (!flowControl) {return;}
    }







    /* 
                                                            ENEMY TURN
    ==================================================================================================================================== 
    */
    bool TryHandleEnemyTurn()
    {
        if (TurnSystemScript.Instance.IsPlayerTurn()) { state = State.WaitingForTurn; return false; }

        switch (state)
        {
            case State.WaitingForTurn:
                // Wait for the player's turn to end
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    state = State.Busy;
                    if (TryInitiateEnemyAIAction(SetStateTakingTurn)) {state = State.Busy;}
                    else {TurnSystemScript.Instance.NextTurn();}
                }
                break;
            case State.Busy:
                // Handle busy state (e.g., animation or cooldown)
                break;
        }
        return true;
    }
    void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
    void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        if (TurnSystemScript.Instance.IsPlayerTurn()) {return;}
        
        state = State.TakingTurn;
        timer = 1f;
    }
    bool TryInitiateEnemyAIAction(Action onEnemyAIActionComplete)
    {
        foreach (Mech enemyMech in MechManager.Instance.GetEnemyMechList())
        {
            if (TryTakeEnemyAIAction (enemyMech, onEnemyAIActionComplete)) {return true;}
        }

        return false;
    }
    bool TryTakeEnemyAIAction(Mech enemyMech, Action onEnemyAIActionComplete)
    {
        SpinAction spinAction = enemyMech.GetSpinAction();
        GridPosition actionGridPosition = enemyMech.GetGridPosition();
        
        if (!spinAction.IsValidActionGridPosition(actionGridPosition)) return false;
        if (!enemyMech.TrySpendCorePowerForAction(spinAction)) return false;

        spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);
        return true;
    }
}
