using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TurnSystemScript : MonoBehaviour
{
    public static TurnSystemScript Instance { get; private set; }

    public event EventHandler OnTurnChange;

    int turnCount = 1;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one TurnSystemScript!" + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void IncreaseTurnCount()
    {
        turnCount++;    
        OnTurnChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnCount()
    {
        return turnCount;
    }
}
