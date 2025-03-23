using System;
using UnityEngine;

public class TurnSystemScript : MonoBehaviour
{
    public static TurnSystemScript Instance { get; private set; }

    public event EventHandler OnTurnEnd;

    int turnCount = 1;




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



    void AdvanceTurnCount()
    {
        turnCount++;
        OnTurnEnd?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnCount()
    {
        return turnCount;
    }

    public void NextTurn()
    {
        AdvanceTurnCount();
        Debug.Log("NextTurn working. Turn " + GetTurnCount());
    }






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
