using System;
using UnityEngine;

public class TurnSystemScript : MonoBehaviour
{
    //core power, heat, and shield handled in the pcmech script
    public static TurnSystemScript Instance { get; private set; }

    public event EventHandler OnTurnEnd;

    int turnCount = 1;
    bool isPlayerTurn = true;



    private void Awake()
    {
        SetInstanceAndDebug();
    }



    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one TurnSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void AdvanceTurnCount()
    {        
        if (!isPlayerTurn)
        {
            turnCount++;
        }

        //only 1 player character means we can just toggle the bool on end turn
        isPlayerTurn = !isPlayerTurn;

        OnTurnEnd?.Invoke(this, EventArgs.Empty);
    }
    public int GetTurnCount() => turnCount;
    public void NextTurn() => AdvanceTurnCount();
    public bool IsPlayerTurn() => isPlayerTurn;






    /* 

    
    public event EventHandler OnTurnStart;



    public void EndTurn()
    {
    }

    public void StartTurn()
    {
        OnTurnStart?.Invoke(this, EventArgs.Empty);
    } */
}
