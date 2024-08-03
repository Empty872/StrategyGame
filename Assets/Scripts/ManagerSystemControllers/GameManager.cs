using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameDefeatUI;
    [SerializeField] private GameObject _gameWinUI;
    public Action OnWin;
    public Action OnDefeat;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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

    public void Defeat()
    {
        _gameDefeatUI.SetActive(true);
        OnDefeat.Invoke();
        Time.timeScale = 0;
    }

    public void Win()
    {
        _gameWinUI.SetActive(true);
        OnWin.Invoke();
        Time.timeScale = 0;
    }
}