using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

//wanna use the constructor so no mono. This object will later have a bunch of use, like having a list of pcmechs on the object, etc.
public class GridObject
{

    private GridPosition gridPosition;
    private GridSystem gridSystem;
    //make it a LIST of mechs to allow for multiple pc units to be on the same cell. Not in a game rule sense, just in a system understands it sense. cells keep updating nice
    private List <PCMech> pCMechList;

    public GridObject (GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        pCMechList = new List <PCMech> ();
    }

    //temp. more data to come
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

    public void AddPcMech (PCMech pcMech)
    {
        pCMechList.Add (pcMech);
    }

    public void RemovePcMech (PCMech pCMech)
    {
        pCMechList.Remove (pCMech);
    }

    public List <PCMech> GetPcMechList()
    {

        return pCMechList;
    }

    public bool HasAnyPcMech()
    {
        return pCMechList.Count > 0;
    }

    //function to show what unit is on the list
    public PCMech GetPCMech()
    {
        if (HasAnyPcMech())
        {
            return pCMechList[0];            
        }
        else
        {
            return null;
        }
    }
}
