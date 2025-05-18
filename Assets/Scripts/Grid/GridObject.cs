using System.Collections.Generic;

//wanna use the constructor so no mono.
public class GridObject
{
    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    //LIST to allow for multiple pc units to be on the same cell.
    private List <PCMech> pCMechList;



    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        pCMechList = new List<PCMech>();
    }
    public override string ToString()
    {
        string pcMechString = "";
        foreach (PCMech pCMech in pCMechList)
        {
            pcMechString += pCMech + "\n";
        }
        //"\n" is new line. this makes it so each cell tells us when a pcMech is in that cell on top of just the numbers
        return gridPosition.ToString() + "\n" + pcMechString;
    }
    public void AddPcMech (PCMech pcMech) {pCMechList.Add (pcMech);}
    public void RemovePcMech (PCMech pCMech) {pCMechList.Remove (pCMech);}
    public List <PCMech> GetPcMechList() {return pCMechList;}
    public bool HasAnyPcMech() {return pCMechList.Count > 0;}
    //function to show what unit is on the list
    public PCMech GetPCMech()
    {
        if (HasAnyPcMech()) {return pCMechList[0];}
        else {return null;}
    }
}
