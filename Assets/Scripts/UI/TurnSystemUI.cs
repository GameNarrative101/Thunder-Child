using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] Button endTurnButton;
    // //if/when a turn counter is implemented, this will be used
    // [SerializeField] TextMeshProUGUI turnCountText;

    void Start()
    {
        endTurnButton.onClick.AddListener(() => TurnSystemScript.Instance.IncreaseTurnCount());

        // //if/when a turn counter is implemented, this will be used
        // TurnSystemScript.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        // UpdateTurnCountText();
    }

        // //if/when a turn counter is implemented, this will be used
    // void TurnSystem_OnTurnChange(object sender, System.EventArgs e)
    // {
    //     UpdateTurnCountText();
    // }

    // //if/when a turn counter is implemented, this will be used
    // void UpdateTurnCountText()
    // {
    //     turnCountText.text = "Turn: " + TurnSystemScript.Instance.GetTurnCount();
    // }
}
