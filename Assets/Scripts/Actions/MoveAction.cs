using UnityEngine;
using System.Collections.Generic;
using System;
public class MoveAction : BaseAction
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] int maxMoveDistance = 4;

    List<Vector3> positionList;
    int currentPositionIndex;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;




    void Update()
    {
        if (!isActive) return;
        
        clickToMove();
    }




/* 
                                                    MOOVIN THANGS
==================================================================================================================================== 
*/
    void clickToMove()
    {

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }

    }






/* 
                                                     OVERRIDES
==================================================================================================================================== 
*/
    public override string GetActionName() => "Move";
    public override void TakeAction (GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList =
            Pathfinding.Instance.FindPath(pCMech.GetGridPosition(), gridPosition, out int pathLength); //where pathfinding happens
        
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List <GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition pcMechGridPosition = pCMech.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = pcMechGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidPosition(testGridPosition)) continue;
                if (pcMechGridPosition == testGridPosition) continue;//unit's own position is invalid
                if (LevelGrid.Instance.HasAnyPcMechOnGridPosition(testGridPosition)) continue; //other unit's positions are invalid
                if (!Pathfinding.Instance.GetIsWalkableGridPosInPath(testGridPosition)) continue; //invalid if the pathfinding node isn't walkable
                if (!Pathfinding.Instance.HasValidPath(pcMechGridPosition, testGridPosition)) continue; //invalid if there's no path to target position

                int pathfindingDistanceMultiplier = 10;
                if (Pathfinding.Instance.GetPathLength(pcMechGridPosition, testGridPosition) >
                    maxMoveDistance *pathfindingDistanceMultiplier) continue; //path too long, position invalid

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
