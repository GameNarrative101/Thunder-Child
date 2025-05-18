using UnityEngine;
using System.Collections.Generic;
using System;

public class MoveAction : BaseAction
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] int maxMoveDistance = 4;

    Vector3 targetPosition;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;






    //using the baseaction awake but changing one thing in it but only in this script's use of it
    protected override void Awake()
    {
        //run the awake on base first, then run this awake after
        base.Awake();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isActive) return;
        
        clickToMove();
    }






/* 
                                                    MOOVIN THANGS
==================================================================================================================================== 
*/
    private void clickToMove()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            ActionComplete();
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);
    }






/* 
                                                     OVERRIDES
==================================================================================================================================== 
*/
    public override string GetActionName() => "Move";
    public override void TakeAction (GridPosition gridPosition, Action onActionComplete)
    {
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        
        ActionStart(onActionComplete);
    }

    //a list of valid grid positions for the action.
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List <GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();

        //we weanna cycle through all valid grid positions. so, for every x from left to right within range and same with every z,
        //gimme a new grid position (offset) so we can add it to current grid position
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition))
                {
                    //basically if the grid position the loop gives us is not valid, we go back and get another one from the loop, otherwise execute the debug
                    continue;
                }
                if (pcMechGridPosition == testGridPosition) 
                {
                    //a valid position can't be where the unit already is
                    continue; 
                }
                if (LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition))
                {
                    //skip all grid positions that have another unit on them. right now this says mech but change if needed
                    continue;
                }
                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }






/* 
                                                      ENEMY MOVE
==================================================================================================================================== 
*/
    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition)
    {
        if (pCMech == null)
        {
            Debug.LogError("pCMech is null in MoveAction.GetBestEnemyAIAction.");
            return null;
        }

        ShootAction shootAction = pCMech.GetAction<ShootAction>();
        if (shootAction == null)
        {
            Debug.LogWarning($"ShootAction is missing on {pCMech.gameObject.name}. Skipping AI action.");
            return null;
        }
    
    
        int TargetCountAtPosition = pCMech.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = TargetCountAtPosition * 10,
        };
    }
}
