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
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void NextTurn()
    {
        AdvanceTurnCount();
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
    public int GetTurnCount()
    {
        return turnCount;
    }
    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
