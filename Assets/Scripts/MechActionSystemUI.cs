using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MechActionSystemUI : MonoBehaviour
{

    [SerializeField] Transform actionButtonPrefab;

    //currently the container is dragged in from the scene, not as a prefab
    [SerializeField] Transform actionButtonContainerTransform;
    
    /* 
       //for selected button visuals. Not needed with current prefabs
    
      List <ActionButtonUI> actionButtonUIList = new List<ActionButtonUI>(); 
    */




    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        CreateMechActionButtons();
        /* 
           //for selected button visuals. Not needed with current prefabs

            unitActionSystem.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
            UpdateSelectedVisual(); 
        */
    }




    void CreateMechActionButtons ()
    {
        //first, destroy all buttons. we look for a transform, then destroy the object attached to it
        foreach (Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        /* 
           //for selected button visuals. Not needed with current prefabs


            //before making the new buttons, clear the list
            actionButtonUIList.Clear(); 
        */


        //get the selected unit
        PCMech selectedMech = UnitActionSystem.Instance.GetSelectedMech();

        //cycle through all actions and create a button for each
        foreach (BaseAction baseAction in selectedMech.GetBaseActionArray())
        {
            //make a button. using a type of instantiate that takes an object and a parent
            //then get the script component from the instantiated button, and run the SetBaseAction function to set the text
            Transform actionButtonTransform = Instantiate (actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponentInChildren<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);

            /* 
               //for selected button visuals. Not needed with current prefabs


                //add each button to the list
                actionButtonUIList.Add(actionButtonUI); 
            */
        }
        
    }

    void UnitActionSystem_OnSelectedUnitChange(object sender, System.EventArgs e)
    {
        CreateMechActionButtons();
        /* 
            //for selected button visuals. Not needed with current prefabs
            UpdateSelectedVisual(); 
        */

    }

    
        /*     
            //for selected button visuals. Not needed with current prefabs


            void UnitActionSystem_OnSelectedActionChange(object sender, System.EventArgs e)
            {
                UpdateSelectedVisual();
            }
            
            void UpdateSelectedVisual()
                { 
                    //get all the buttons and update their visuals. the updating happens on actionbuttonui
                    foreach (ActionButtonUI actionButtonUI in actionButtonUIList)
                    { 
                        actionButtonUI.UpdateSelectedVisual();
                    }
                } 
        */

}
