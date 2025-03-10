using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class MoveAction : BaseAction
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] int maxMoveDistance = 4;
    [SerializeField] Animator pcmechAnimator;

    Vector3 targetPosition;



    //using the baseaction awake but changing one thing in it but only in this script's use of it
    protected override void Awake()
    {
        //run the awake on base first, then run this awake after
        base.Awake();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isActive){return;}
        
        clickToMove();
    }


    private void clickToMove()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;


            pcmechAnimator.SetBool("IsWalking", true);
        }
        else
        {
            pcmechAnimator.SetBool("IsWalking", false);
            isActive = false;
            onActionComplete();
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotationSpeed * Time.deltaTime);

    }

    public void move(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        isActive = true;
    }

    
    public bool IsValidActionGridPosition (GridPosition gridPosition)
    {
        List <GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    //a list of grid positions for all the valid actions in this action
    public List<GridPosition> GetValidActionGridPositionList()
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

    public override string GetActionName()
    {
        return "Move";
    }
}
