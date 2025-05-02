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
        if (TurnSystemScript.Instance.IsPlayerTurn())
        {
            return;
        }
        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryActivateEnemyAI(SetStateTakingTurn)) {state = State.Busy;}
                    else {TurnSystemScript.Instance.NextTurn();}
                }
                break;
            case State.Busy:
                break;
        }
    }






    private void TurnSystemScript_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystemScript.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }
    bool TryActivateEnemyAI(Action onEnemyAIActionComplete)
    {
        foreach (PCMech enemyUnit in UnitManager.Instance.GetEnemyMechList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) {return true;}
        }
        return false;
    }

    bool TryTakeEnemyAIAction(PCMech enemyUnit, Action onEnemyAIActionComplete)
    {
        SpinAction spinAction = enemyUnit.GetComponent<SpinAction>();

        GridPosition actionGridPosition = enemyUnit.GetGridPosition();
        if (!spinAction.IsValidActionGridPosition(actionGridPosition)) return false;
        if (!enemyUnit.TrySpendCorePowerForAction(spinAction)) return false;

        spinAction.TakeAction(actionGridPosition, onEnemyAIActionComplete);
        return true;
    }

    void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }
}
