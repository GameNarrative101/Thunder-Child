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
    [SerializeField] GameObject enemyTurnPanel;
    [SerializeField] GameObject actionBar;
    [SerializeField] GameObject resourceBars;
    [SerializeField] GameObject endTurnUI;





    void Start()
    {
        TurnSystemScript.Instance.OnTurnEnd += TurnSystemScript_OnTurnEnd;
        // TurnSystemScript.Instance.OnTurnStart += TurnSystemScript_OnTurnStart;

        SetEndTurnButton();
        UpdateTurnCountText();
        UpdateEnemyTurnPanel();
    }

    void Update()
    {
        //TESTING ONLY
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TurnSystemScript.Instance.NextTurn();
        }
    }





    void SetEndTurnButton()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystemScript.Instance.NextTurn();
        });


    }
    
    private void TurnSystemScript_OnTurnEnd(object sender, EventArgs e)
    {
        UpdateTurnCountText();
        UpdateEnemyTurnPanel();
    }
    
    void UpdateTurnCountText()
    {
        turnCountText.text = "Turn " + TurnSystemScript.Instance.GetTurnCount();
    }

        void UpdateEnemyTurnPanel()
    {
        enemyTurnPanel.SetActive (!TurnSystemScript.Instance.IsPlayerTurn());
        endTurnUI.SetActive (TurnSystemScript.Instance.IsPlayerTurn());
        actionBar.SetActive (TurnSystemScript.Instance.IsPlayerTurn());
        resourceBars.SetActive (TurnSystemScript.Instance.IsPlayerTurn());
    }
}
