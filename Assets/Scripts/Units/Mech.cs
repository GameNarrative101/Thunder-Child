using System;
using UnityEngine;

public abstract class Mech : MonoBehaviour
{
        //all grid position stuff are here for the sake of forced movement handling
    GridPosition gridPosition;
    HealthSystem healthSystem;
    BaseAction[] baseActionArray;    
    MoveAction moveAction;
    SpinAction spinAction;

    bool isDead = false;

    public static event EventHandler OnAnyMechSpawned;
    public static event EventHandler OnAnyMechDead;
    //static makes it so any instance of this class in other classes can change things, and all instances will be updated
   






    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        SetGridPosition();

        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
        healthSystem.OnDead += HealthSystem_OnDead;
        OnAnyMechSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GetNewGridPositionAndUpdate();
    }





/* 
                                                    GRID FUNCTIONS
==================================================================================================================================== 
*/
    protected void SetGridPosition()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddMechAtGridPosition(gridPosition, this);
    }
    protected void GetNewGridPositionAndUpdate()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.MechMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }






/* 
                                                    ABSTRACT FUNCTIONS
==================================================================================================================================== 
*/
    public abstract void TurnSystemScript_OnTurnEnd(object sender, EventArgs e);
    






/* 
                                                    VIRTUAL FUNCTIONS
===================================================================================================================================== 
*/
    public virtual GridPosition GetGridPosition() => gridPosition;
    public virtual Vector3 GetWorldPosition() => transform.position;
    public virtual bool GetIsDead() => isDead;

    public virtual void TakeDamage(int damageAmount) {healthSystem.Damage(damageAmount);}

    public virtual void HealthSystem_OnDead (object sender, EventArgs e)
    {
        isDead=true;
        LevelGrid.Instance.RemoveMechAtGridPosition(gridPosition, this);

        //add death animation, then destroy
        Destroy(gameObject);

        OnAnyMechDead?.Invoke(this, EventArgs.Empty);
    }
}
