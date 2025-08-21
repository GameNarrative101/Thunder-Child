using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystemScript : MonoBehaviour
{
    public static TurnSystemScript Instance { get; private set; }
    public event EventHandler OnTurnEnd;

    [SerializeField] private PCMech playerMech;
    List<PCMech> initiativeMechList = new List<PCMech>();
    int turnCount = 1;
    int roundCount = 1;
    int currentInitiativeIndex = 0;
    bool isPlayerTurn = true;



    private void Awake()
    {
        SetInstanceAndDebug();
    }
    private void Start()
    {
        initiativeMechList = BuildInitiativeOrder(playerMech);

        foreach (PCMech mech in UnitManager.Instance.GetMechList())
        {
            mech.GetComponent<HealthSystem>().OnDead += HealthSystem_OnDead; //For adhjsting initiative order when a mech dies
        }
    }



    private void SetInstanceAndDebug()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    List<PCMech> BuildInitiativeOrder(PCMech playerMech)
    {
        initiativeMechList.Clear();

        List<PCMech> enemyList = UnitManager.Instance.GetEnemyMechList();

        System.Random rng = new System.Random(); //randomize enemy order
        int n = enemyList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            PCMech value = enemyList[k];
            enemyList[k] = enemyList[n];
            enemyList[n] = value;
        }

        foreach (PCMech enemy in enemyList)
        {
            initiativeMechList.Add(playerMech);
            initiativeMechList.Add(enemy);
        }
        return initiativeMechList;
    }
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        HealthSystem hs = sender as HealthSystem;
        if (hs == null) return;
        
        PCMech deadMech = hs.GetComponent<PCMech>();
        if (deadMech == null) return;

        int mechIndex = initiativeMechList.IndexOf(deadMech);
        if (mechIndex == -1) return;

        // Remove the dead mech
        initiativeMechList.RemoveAt(mechIndex);

        // If there's a mech BEFORE it, and it's the player, remove that too
        int playerIndex = mechIndex - 1;
        if (playerIndex < 0) return;
        if (initiativeMechList[playerIndex] == playerMech)
        {
            initiativeMechList.RemoveAt(playerIndex);
        }
    }
    void AdvanceTurn()
    {
        if (initiativeMechList.Count == 0)
        {
            Debug.LogWarning("Initiative list is empty!");
            return;
        }

        // advance initiative index
        currentInitiativeIndex++;
        if (currentInitiativeIndex >= initiativeMechList.Count)
        {
            currentInitiativeIndex = 0;
            roundCount++; // full round completed
        }

        PCMech currentMech = initiativeMechList[currentInitiativeIndex];

        // update isPlayerTurn
        isPlayerTurn = !currentMech.GetIsEnemy();

        if (isPlayerTurn)
        {
            turnCount++;
        }

        OnTurnEnd?.Invoke(this, EventArgs.Empty);

    }




    //==================================================================================================================================== 
    #region GETTERS & PUBLIC METHODS
    public bool GetIsPlayerTurn()
    {
        return isPlayerTurn;
    }
    public void NextTurn()
    {
        AdvanceTurn();
    }
    public List<PCMech> GetInitiativeOrder()
    {
        return initiativeMechList;
    }
    public PCMech GetCurrentMechOnInitiative()
    {
        return initiativeMechList[currentInitiativeIndex];
    }
    public int GetTurnCount()
    {
        return turnCount;
    }
    public int GetRoundCount() //not used yet
    {
        return roundCount;
    }

    #endregion
}