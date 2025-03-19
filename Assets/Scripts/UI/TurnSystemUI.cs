using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] Button endTurnButton;
    [SerializeField] TextMeshProUGUI turnCountText;

    void Start()
    {
        endTurnButton.onClick.AddListener(() => TurnSystemScript.Instance.IncreaseTurnCount());

        TurnSystemScript.Instance.OnTurnChange += TurnSystem_OnTurnChange;
        UpdateTurnCountText();
    }

    void TurnSystem_OnTurnChange(object sender, System.EventArgs e)
    {
        UpdateTurnCountText();
    }

    void UpdateTurnCountText()
    {
        turnCountText.text = "Turn: " + TurnSystemScript.Instance.GetTurnCount();
    }
}
