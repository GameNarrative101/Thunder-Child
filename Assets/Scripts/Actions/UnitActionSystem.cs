using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set;}
    
    [SerializeField] PCMech selectedPcMech;
    [SerializeField] LayerMask unitLayerMask;
    
    bool isBusy;
    BaseAction selectedAction;

    //there's a missing event OnSelectedActionChange. just in case something doesn't work! 
    public event EventHandler OnSelectedUnitChange;
    public event EventHandler <bool> onBusyChanged;

    private void Awake()
    {
        SetInstanceAndDebug();
    }

    void Start()
    {
        SetSelectedPcMech(selectedPcMech);
    }


    private void Update()
    {
        UnitActionOperation();
    }

    private void UnitActionOperation()
    {
        if (isBusy) {return;}
        
        //if the mouse is over a UI element, don't do anything
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;          
        }
        if (TryHandleUnitSelection()) {return;}
        HandleSelectedAction();
    }

    void SetBusy()
    {
        isBusy = true;

        onBusyChanged?.Invoke(this, isBusy);
    }
    void ClearBusy()
    {
        isBusy = false;

        onBusyChanged?.Invoke(this, isBusy);

    }

    void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);
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

    //on mouse button being down, see if it's on top of a unit. if it is, select that unit
    bool TryHandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
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
                    if (pcMech == selectedPcMech)
                    {
                        //we have already selected this unit.
                        return false;
                    }
                    SetSelectedPcMech (pcMech);
                    return true;
                }
            }
        }
       return false;
       
    }

    void SetSelectedPcMech(PCMech pcMech)
    {
        selectedPcMech = pcMech;
        //defaults to move action on selecting a unit. all units get a move action
        SetSelectedAction(pcMech.GetMoveAction());
        
        //a simpler way to do what is just an if statement checkign to see if seleced unit has changed or not
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
    }

   //important to select a unit without having to assign it on every script
    public PCMech GetSelectedMech()
    {
        return selectedPcMech;
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
