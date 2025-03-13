using System;
using System.ComponentModel;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

//subscribes to the action system's onBusyChanged event and changes the ui to show it's in use
public class ActionBusyUI : MonoBehaviour
{

    public event EventHandler <bool> onBusyActivated;

    void Start()
    {
        UnitActionSystem.Instance.onBusyChanged += UnitActionSystem_onBusyChanged;

        Hide();
    }

   //unsubscribe from the event before destroying the button so it doesn't keep calling and throw an error
    void OnDestroy()
    {
        UnitActionSystem.Instance.onBusyChanged -= UnitActionSystem_onBusyChanged;
    }


    void Show()
    {
        gameObject.SetActive(true);

        onBusyActivated?.Invoke(this, true);
    }

    void Hide()
    {
        gameObject.SetActive(false);

        onBusyActivated?.Invoke(this, false);

    }

    void UnitActionSystem_onBusyChanged(object sender, bool isBusy)
    {
        if (isBusy)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }


  
}
