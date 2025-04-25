using System;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class PCMech : MonoBehaviour
{
    //all grid position stuff are here for the sake of forced movement handling
    GridPosition gridPosition;
    HealthSystem healthSystem;
    BaseAction[] baseActionArray;    
    MoveAction moveAction;
    SpinAction spinAction;

    bool isDead = false;
    [SerializeField] bool isEnemy;
    
    int corePower = 3;
    [SerializeField] int corePowerIncrease = 3;
    [SerializeField] int maxCorePower = 15;
    [SerializeField] int heat = 0;
    [SerializeField] int maxHeat = 10;

    /* 
        OnTurnEnd is fired from different scripts, so execution order can cause bugs. 
        Instead, static makes it so this fires whenever any instance of the class changes corePower, but only in this class.  
    */
    public static event EventHandler OnCorePowerChange;
    public static event EventHandler OnAnyHeatChange;




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




    public MoveAction GetMoveAction() {return moveAction;}
    public SpinAction GetSpinAction() {return spinAction;}
    public GridPosition GetGridPosition() {return gridPosition;}
    public BaseAction[] GetBaseActionArray() {return baseActionArray;}




    /*
        checking if the pcmech has enough core power for the action

        possible that a heat equivalent is needed if heat turns out to be a resource that can be spent this way
        alternatively, this might be a way to handle passive heat abilities. 
    */

    /* 
        instead of making it impossible to go below 0, we make it impossible to spend power you don't have (on UnitActionSystem) 
    */
    void SpendCorePower (int amount)
    {
        corePower -= amount;
        if (corePower > maxCorePower)
        {
            print("Overloaded!");
        }
    }
    public void GainHeat (int amount) //effects of overheating to be implemented later
    {
        heat += amount;
        if (heat > maxHeat)
        {
            print("Overheated!");
        }
        OnAnyHeatChange?.Invoke(this, EventArgs.Empty);
    }
    void ReduceHeat (int amount)
    {
        heat -= amount;
        if (heat < 0)
        {
            heat = 0;
        }
        OnAnyHeatChange?.Invoke(this, EventArgs.Empty);
    }
    public void TryReduceHeat (int amount) //for other scripts' use
    {
        if (heat - amount >= 0)
        {
            ReduceHeat(amount);
        }
        else
        {
            print("Not enough heat to reduce!");
        }
    }
    
    public bool TrySpendCorePowerForAction(BaseAction baseAction) //for other scripts' use
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
    public void TakeDamage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }




    public int GetCorePower()
    {
        return corePower;
    }
    public int GetHeat()
    {
        return heat;
    }
    public float GetCorePowerNormalized()
    {
        return corePower / (float)maxCorePower;
    }
    public float GetHeatNormalized()
    {
        return heat / (float)maxHeat;
    }
    public UnityEngine.Vector3 GetWorldPosition()
    {
        return transform.position;
    }
    public bool IsEnemy()
    {
        return isEnemy;
    }
    public bool IsDead()
    {
        Debug.Log ("isDead = " + isDead);
        return isDead;
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




    private void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        //increase core power at the beginning of the player's turn
        if ((IsEnemy() && !TurnSystemScript.Instance.IsPlayerTurn()) || 
        (!IsEnemy() && TurnSystemScript.Instance.IsPlayerTurn()))
        {
            corePower += corePowerIncrease;
            OnCorePowerChange?.Invoke(this, EventArgs.Empty);
        }

        /* //What happens when core power goes above max
            
            if (corePower > maxCorePower)
        {
            corePower = maxCorePower;
        }
         */
    }
    void HealthSystem_OnDead (object sender, EventArgs e)
    {
        isDead=true;
        LevelGrid.Instance.RemoveMechAtGridPosition(gridPosition, this);

        //add death animation, then destroy
        Destroy(gameObject);
    }
}
