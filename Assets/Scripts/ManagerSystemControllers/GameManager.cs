using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameDefeatUI;
    [SerializeField] private GameObject _gameWinUI;

    private void Start()
    {
        UnitManager.Instance.OnAllEnemiesDied += UnitManager_OnAllEnemiesDied;
        UnitManager.Instance.OnAllFriendlyUnitsDied += UnitManager_OnAllFriendlyUnitsDied;
    }

    private void UnitManager_OnAllFriendlyUnitsDied(object sender, EventArgs e)
    {
        Defeat();
    }

    private void UnitManager_OnAllEnemiesDied(object sender, EventArgs e)
    {
        Win();
    }

    private void Defeat()
    {
        _gameDefeatUI.SetActive(true);
    }

    private void Win()
    {
        _gameWinUI.SetActive(true);
    }
}