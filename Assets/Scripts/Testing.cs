using UnityEngine;
using UnityEngine.InputSystem;

//empty for now
public class Testing : MonoBehaviour
{

    [SerializeField] PCMech pcMech;
    
    
    void Start()
    {
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridSystemVisuals.Instance.HideAllGridPosition();
            GridSystemVisuals.Instance.ShowGridPositionList(pcMech.GetMoveAction().GetValidActionGridPositionList());
        }
    }


}
