using System;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class PCMech : MonoBehaviour
{
    [SerializeField] bool isEnemy;
    bool isDead = false;
    
    //all grid position stuff are here for the sake of forced movement handling
    GridPosition gridPosition;
    //will be storing all actions in baseaction later
    HealthSystem healthSystem;
    MoveAction moveAction;
    SpinAction spinAction;
    BaseAction[] baseActionArray;
    
    /* 
        OnTurnEnd is fired from different scripts, so execution order can cause bugs. 
        Instead, static makes it so this fires whenever any instance of the class changes corePower, but only in this class.  
    */
    public static event EventHandler OnAnyCorePowerChange;
    // public static event EventHandler OnAnyHeatChange;

    int corePower = 3;
    [SerializeField] int corePowerIncrease = 3;
    // int maxCorePower = 15;
    // int heat = 0;
    // [SerializeField] int heatDecrease = 3;
    // int maxHeat = 15;
    // int shield = 0;    





    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        //plural getcomponents to get all components that extend baseaction (all actions)
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        //so on start, this script calculates its own grid position then calls the setmechatgridposition function in levelgrid
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddMechAtGridPosition(gridPosition, this);

        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;

        healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void Update()
    {
        GetNewGridPosition();
    }





    private void GetNewGridPosition()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.MechMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction() {return moveAction;}
    public SpinAction GetSpinAction() {return spinAction;}
    public GridPosition GetGridPosition() {return gridPosition;}
    public BaseAction[] GetBaseActionArray() {return baseActionArray;}


    private void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        //increase core power at the beginning of the player's turn
        if ((IsEnemy() && !TurnSystemScript.Instance.IsPlayerTurn()) || 
        (!IsEnemy() && TurnSystemScript.Instance.IsPlayerTurn()))
        {
            corePower += corePowerIncrease;
            OnAnyCorePowerChange?.Invoke(this, EventArgs.Empty);
        }

        /* //What happens when core power goes above max
            
            if (corePower > maxCorePower)
        {
            corePower = maxCorePower;
        }
         */
    }



    /*
        checking if the pcmech has enough core power for the action

        possible that a heat equivalent is needed if heat turns out to be a resource that can be spent this way
        alternatively, this might be a way to handle passive heat abilities. 
    */
    public bool CanSpendCorePowerForAction (BaseAction baseAction)
    {
        if (corePower >= baseAction.GetCorePowerCost())
        {
            return true; 
        }
        else
        {
            return false;
        }
    }

    /* 
        instead of making it impossible to go below 0, we make it impossible to spend power you don't have (on UnitActionSystem) 
    */
    void SpendCorePower (int amount)
    {
        corePower -= amount;
        OnAnyCorePowerChange?.Invoke(this, EventArgs.Empty);
    }

    //exposes the above 2 functions to other scripts
    public bool TrySpendCorePowerForAction(BaseAction baseAction)
    {
        if (CanSpendCorePowerForAction(baseAction))
        {
            SpendCorePower(baseAction.GetCorePowerCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetCorePower()
    {
        return corePower;
    }

    public UnityEngine.Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void TakeDamage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    void HealthSystem_OnDead (object sender, EventArgs e)
    {
        isDead=true;

        LevelGrid.Instance.RemoveMechAtGridPosition(gridPosition, this);

        //add death animation, then destroy
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        Debug.Log ("isDead = " + isDead);
        return isDead;
    }
}
