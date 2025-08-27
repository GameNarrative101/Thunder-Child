using System;
using System.Collections.Generic;
using UnityEngine;

public class LaserMinigunAction : BaseAction
{
    enum State { Aiming, Shooting, Cooloff }

    State state;
    PCMech targetUnit;
    bool canShootBullet;
    float stateTimer;

    //once tested and confirmed, store each state's timer locally
    [SerializeField] int maxShootDistance = 5;
    [SerializeField] Vector3Int enemyDamageByTier = new Vector3Int(4, 5, 7);
    [SerializeField] Vector3Int playerDamageByTier = new Vector3Int(12, 15, 21);
    [SerializeField] int heatGenerated = 1;
    [SerializeField] int corePowerCost = 1;
    [SerializeField] float aimingStateTime = 0.5f;
    [SerializeField] float shootingStateTime = 0.1f;
    [SerializeField] float coolOffStateTime = 0.5f;
    [SerializeField] LayerMask obstaclesLayerMask;

    public event EventHandler<OnShootEventArgs> OnShoot;
    public static event EventHandler<OnShootEventArgs> OnAnyShoot; //for scren shake

    public class OnShootEventArgs : EventArgs //adding more to the event, instead of <> on event handler. another option
    {
        public PCMech targetMech;
        public PCMech shootingUnit;
    }



    private void Update()
    {
        bool flowControl = HandleShooting();
        if (!flowControl) { return; }
    }



    //==================================================================================================================================== 
    #region SHOOTIN' THANGS

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();
        return GetValidActionGridPositionList(pcMechGridPosition);
    }
    public List<GridPosition> GetValidActionGridPositionList(GridPosition pcMechGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        //we weanna cycle through all valid grid positions. so, for every x from left to right within range and same with every z,
        //gimme a new grid position (offset) so we can add it to current grid position
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;// Skip invalid grid positions
                if (!LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition)) continue;// Skip grid positions without any units

                PCMech targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(testGridPosition);
                if (targetUnit.GetIsEnemy() == pCMech.GetIsEnemy()) continue;// Skip grid positions with allied units

                Vector3 pcMechWorldPosition = LevelGrid.Instance.GetWorldPosition(pcMechGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - pcMechWorldPosition).normalized;
                float pcMechShoulderHeight = 1.7f;
                if (Physics.Raycast(pcMechWorldPosition + Vector3.up * pcMechShoulderHeight,
                        shootDir,
                        Vector3.Distance(pcMechWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask))
                {
                    continue; // Skip grid positions with obstacles in the way
                }

                validGridPositionList.Add(testGridPosition);// Add valid grid position to the list
            }
        }
        return validGridPositionList;
    }
    public override void TakeAction(GridPosition gridPosition, Action clearBusyOnActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetPcMechAtGridPosition(gridPosition);
        // HealthSystem targetHealthSystem = targetUnit.GetComponent<HealthSystem>();

        state = State.Aiming;
        stateTimer = aimingStateTime;

        canShootBullet = true;
        if (pCMech.GetIsEnemy()) EnemyActionTaken = true;

        ActionStart(clearBusyOnActionComplete);
    }
    private bool HandleShooting()
    {
        if (!isActive) { return false; }
        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - pCMech.GetWorldPosition()).normalized; //rotate towards target
                float rotationSpeed = 30f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, rotationSpeed * Time.deltaTime);
                break;
            case State.Shooting:
                if (canShootBullet)
                {
                    ShootBullet();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f) { NextState(); }

        return true;
    }
    private void NextState()
    {
        switch (state)
        {
            //going through the first state happens on the TakeAction function
            case State.Aiming:
                state = State.Shooting;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }
    private void ShootBullet()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs { targetMech = targetUnit, shootingUnit = pCMech });
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { targetMech = targetUnit, shootingUnit = pCMech });

        var (rolledDamage, _, _) = GetRolledDamageAndKnockback();
        targetUnit.TakeDamage(rolledDamage);
    }

    #endregion



    //==================================================================================================================================== 
    #region SETTIN' & GETTIN'

    public override string GetActionName() => "Laser Minigun";
    protected override (int, int, int) GetDamageByTier()
    {
       if (!pCMech.GetIsEnemy())
       {
           return (playerDamageByTier.x, playerDamageByTier.y, playerDamageByTier.z);
       }
       else
       {
           return (enemyDamageByTier.x, enemyDamageByTier.y, enemyDamageByTier.z);
       }
    }
    public override int GetCorePowerCost() => corePowerCost;
    public override int GetHeatGenerated() => heatGenerated;
    public int GetMaxShootDistance() => maxShootDistance;
    public PCMech GetTargetUnit() => targetUnit;

    #endregion



    //==================================================================================================================================== 
    #region ENEMY SHOOTIN THANGS

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition simulatedPosition)
    {
        List<GridPosition> targetList = GetValidActionGridPositionList(simulatedPosition);
        if (targetList.Count == 0) return null;

        return new EnemyAIAction
        {
            gridPosition = simulatedPosition,
            actionValue = 50,
        };
    }
    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    } 
    
    #endregion

}
