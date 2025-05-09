using System;
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

    public static event EventHandler OnCorePowerChange;
    public static event EventHandler OnHeatChange;
    public static event EventHandler OnAnyMechSpawned;
    public static event EventHandler OnAnyMechDead;
    //static makes it so any instance of this class in other classes can change things, and all instances will be updated
   






    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
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
    void SetGridPosition()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddMechAtGridPosition(gridPosition, this);
    }
    void GetNewGridPositionAndUpdate()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.MechMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }






/* 
                                                    RESOURCE FUNCTIONS
==================================================================================================================================== 
*/
    void SpendCorePower (int amount) // instead of making it impossible to go below 0, we make it impossible to spend power you don't have (on UnitActionSystem)
    {
        corePower -= amount;
        OnCorePowerChange?.Invoke(this, EventArgs.Empty);
        if (corePower > maxCorePower) {print("Overloaded!");}
    }
    void ReduceHeat (int amount)
    {
        heat -= amount;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
        if (heat < 0) {heat = 0;}
    }
    





/* 
                                                    SUBSCRIPTION FUNCTIONS
==================================================================================================================================== 
*/
    void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        //increase core power at the beginning of the player's turn
        if ((IsEnemy() && !TurnSystemScript.Instance.IsPlayerTurn()) || 
        (!IsEnemy() && TurnSystemScript.Instance.IsPlayerTurn()))
        {
            corePower += corePowerIncrease;
            OnCorePowerChange?.Invoke(this, EventArgs.Empty);

            TryReduceHeat(2);
            OnHeatChange?.Invoke(this, EventArgs.Empty);
        }
    }
    void HealthSystem_OnDead (object sender, EventArgs e)
    {
        isDead=true;
        LevelGrid.Instance.RemoveMechAtGridPosition(gridPosition, this);

        //add death animation, then destroy
        Destroy(gameObject);

        OnAnyMechDead?.Invoke(this, EventArgs.Empty);
    }






/* 
                                                    PUBLIC FUNCTIONS
===================================================================================================================================== 
*/
    public bool IsEnemy() => isEnemy;
    public bool IsDead() => isDead;


    public void TakeDamage(int damageAmount) {healthSystem.Damage(damageAmount);}


    public bool CanSpendCorePowerForAction (BaseAction baseAction) {return corePower >= baseAction.GetCorePowerCost();}
    public bool TrySpendCorePowerForAction(BaseAction baseAction) //for other scripts' use
    {
        if (CanSpendCorePowerForAction(baseAction))
        {
            SpendCorePower(baseAction.GetCorePowerCost());
            return true;
        }
        return false;
    }
    public void GainHeat (int amount) //effects of overheating to be implemented later
    {
        heat += amount;
        OnHeatChange?.Invoke(this, EventArgs.Empty);
        if (heat > maxHeat) {print("Overheated!");}
    }
    public void TryReduceHeat (int amount) //for other scripts' use
    {
        if (heat - amount >= 0) {ReduceHeat(amount);}
        print("Not enough heat to reduce!");
    }
    





/* 
                                                    GETTING THINGS
===================================================================================================================================== 
*/
    public BaseAction[] GetBaseActionArray() => baseActionArray;
    public MoveAction GetMoveAction() => moveAction;
    public SpinAction GetSpinAction() => spinAction;
    
    public GridPosition GetGridPosition() => gridPosition;
    public UnityEngine.Vector3 GetWorldPosition() => transform.position;
    
    public int GetCorePower() => corePower;
    public float GetCorePowerNormalized() => corePower / (float)maxCorePower;
    
    public int GetHeat() => heat;
    public float GetHeatNormalized() => heat / (float)maxHeat;
}
