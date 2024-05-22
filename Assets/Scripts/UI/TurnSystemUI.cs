using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private TextMeshProUGUI _turnNumberText;
    [SerializeField] private GameObject _enemyTurnVisualGameObject;

    void Start()
    {
        _endTurnButton.onClick.AddListener(TurnSystem.Instance.NextTurn);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisual();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisual();
    }

    private void UpdateTurnText()
    {
        _turnNumberText.text = "Turn: " + TurnSystem.Instance.TurnNumber;
    }

    // Update is called once per frame
    private void UpdateEnemyTurnVisual()
    {
        _enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn);
    }

    private void UpdateEndTurnButtonVisual()
    {
        _endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn);
    }
}