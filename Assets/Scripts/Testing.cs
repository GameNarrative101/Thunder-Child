using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    [SerializeField] PCMech pcMech;  
    [SerializeField] GameObject particleBeamPrefab; // Drag your ParticleBeam prefab here in the Inspector
    ParticleBeam activeBeam;
    void Start()
    {
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            pcMech.TakeDamage(150);
            /* CHECKING PATHFINDING
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridPosition startGridPosition = new GridPosition(0, 0);

            List<GridPosition> gridPositionList = Pathfinding.Instance.FindPath(startGridPosition, mouseGridPosition);

            for (int i = 0; i < gridPositionList.Count -1; i++)
            {
                Debug.DrawLine(LevelGrid.Instance.GetWorldPosition(gridPositionList[i]),
                    LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]), Color.green, 10f);
            } 
            */
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            BaseAction baseAction = pcMech.GetComponent<BaseAction>();
            Debug.Log(baseAction.GetPlayerActionTaken());
        }

        if (Input.GetKeyDown(KeyCode.O))
            {
                var order = TurnSystemScript.Instance.GetInitiativeOrder();
                Debug.Log(string.Join(", ", order));
            }   
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TurnSystemScript.Instance.NextTurn();
        }
    }
}
