using Unity.VisualScripting;
using UnityEngine;
using System;

public class UnitActionSystem : MonoBehaviour
{
    [SerializeField] PCMech selectedPcMech;
    [SerializeField] LayerMask unitLayerMask;

    public static UnitActionSystem Instance { get; private set;}
    public event EventHandler OnSelectedUnitChange;
    bool isBusy;

    private void Awake()
    {
        SetInstanceAndDebug();
    }


    private void Update()
    {
        if (isBusy) {return;}
        MovingMech();
        SpinningMech();
    }

    void SetBusy()
    {
        isBusy = true;
    }
    void ClearBusy()
    {
        isBusy = false;
    }

    private void SpinningMech()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetBusy();
            //we define a ClearBusy function here, and because we gave the Spin function in SpinAction a delegate box, we get to put whatever function we want in that box.
            //here, we wanna know when this is done, so we can declare the action done and be able to do othert things.
            selectedPcMech.GetSpinAction().Spin(ClearBusy);
        }
    }

    //This is temp. it will be replaced with a proper move action with grid later
    //The 2nd if statement is not temp
    private void MovingMech()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TryHandleUnitSelection()) return;

            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedPcMech.GetMoveAction().IsValidActionGridPosition(mouseGridPosition))
            {
                SetBusy();
                selectedPcMech.GetMoveAction().move(mouseGridPosition, ClearBusy);
            }
        }
    }

    private void SetInstanceAndDebug()
    {
        if (Instance != null)
        {
            Debug.LogError("there's more than one UnitActionSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    bool TryHandleUnitSelection()
    {
        //get the camera to point a Ray called ray at where the mouse is pointing.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //does the raycast system hit anything at the layer defined above and at any distance? if so, move on
       if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
        {
            //is the thing raycast hit a PC mech?
            //If things that are not named pc mech are to be selected, <PCMech> is what needs to change or be added to
            //(pcMech here is just a PUBLIC SCRIPT COMPONENT WE ATTACH TO ANYTHING WE WANT) the "out" here is a boolian so we just get an answer
            if (raycastHit.transform.TryGetComponent<PCMech>(out PCMech pcMech))
            //Alternative to the line above would be:
            //PCMech pcMech = raycastHit.transform.GetComponent<PCMech>();
            //if (pcMech != null) then run the brackets
            {
                SetSelectedUnit (pcMech);
                return true;
            }
        }
       return false;
       
    }

    void SetSelectedUnit(PCMech pcMech)
    {
        selectedPcMech = pcMech;
        
        //a simpler way to do what is commented out below. the idea here is just an if statement checkign to see if
        //seleced unit has changed or not
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
        
        //    if (OnSelectedUnitChange != null)
        //    {
        //    OnSelectedUnitChange(this, EventArgs.Empty);
        //    }
        
    }

   //important to select a unit without having to assign it on every script
    public PCMech GetSelectedMech()
    {
        return selectedPcMech;
    }

}
