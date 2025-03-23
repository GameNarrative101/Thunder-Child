using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    /* 

    right now the turn count and core power go up when end turn button is clilcked. 
    core power going up is good, but maybe turn count on turn start instead of end?
    in that case, introduce an OnTurnStart event in TurnSystemScript and attach it to the turn count increment.
    
    */ 
    
    [SerializeField] TextMeshProUGUI turnCountText;
    [SerializeField] Button endTurnButton;




    void Start()
    {
        SetEndTurnButton();
        UpdateTurnCountText();
        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
        // TurnSystemScript.Instance.OnTurnStart += TurnSystemScript_OnTurnStart;
    }





    void SetEndTurnButton()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystemScript.Instance.NextTurn();
        });
    }

    void UpdateTurnCountText()
    {
        turnCountText.text = "Turn " + TurnSystemScript.Instance.GetTurnCount();
    }

    private void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        UpdateTurnCountText();
    }
}
