using System;
using System.Collections.Generic;
using UnityEngine;

public class MechManager : MonoBehaviour
{
    public static MechManager Instance {get; private set;}
    
    List <PCMech> mechList;
    List <PCMech> friendlyMechList;
    List <PCMech> enemyMechList;



    void Awake()
    {
        SetInstanceAndDebug();

        mechList = new List<PCMech>();
        friendlyMechList = new List<PCMech>();
        enemyMechList = new List<PCMech>();
    }
    void Start()
    {
        PCMech.OnAnyUnitSpawned += PCMech_anyUnitSpawned;
        PCMech.OnAnyUnitDead += PCMech_anyUnitDied;
    }



    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one MechManager" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void PCMech_anyUnitSpawned(object sender, EventArgs e)
    {
        PCMech pcMech = sender as PCMech;
        mechList.Add(pcMech);

        if (pcMech.IsEnemy()) {enemyMechList.Add(pcMech);}
        else {friendlyMechList.Add(pcMech);}
    }
    private void PCMech_anyUnitDied(object sender, EventArgs e)
    {
        PCMech pcMech = sender as PCMech;
        mechList.Remove(pcMech);

        if (pcMech.IsEnemy()) {enemyMechList.Remove(pcMech);}
        else {friendlyMechList.Remove(pcMech);}    
    }
    
    public List<PCMech> GetMechList() { return mechList; }
    public List <PCMech> GetFriendlyMechList() {return friendlyMechList;}
    public List <PCMech> GetEnemyMechList() {return enemyMechList;}
    public PCMech GetFriendlyMechAtGridPosition(GridPosition gridPosition)
    {
        foreach (PCMech pcMech in friendlyMechList)
        {
            if (pcMech.GetGridPosition() == gridPosition) {return pcMech;}
        }
        return null;
    }
    public PCMech GetEnemyMechAtGridPosition(GridPosition gridPosition)
    {
        foreach (PCMech pcMech in enemyMechList)
        {
            if (pcMech.GetGridPosition() == gridPosition) {return pcMech;}
        }
        return null;
    }
}
