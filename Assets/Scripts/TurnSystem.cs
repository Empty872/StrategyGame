using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TurnSystem : MonoBehaviour
{
    public int TurnNumber { get; private set; } = 1;
    private bool _isPlayerTurn = true;
    public bool IsPlayerTurn => _isPlayerTurn;
    public static TurnSystem Instance { get; private set; }
    public event EventHandler<OnTurnChangedEventArgs> OnTurnChanged;

    public class OnTurnChangedEventArgs : EventArgs
    {
        public bool isPlayerTurn;
    }


    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NextTurn()
    {
        if (!_isPlayerTurn)
        {
            TurnNumber++;
        }
        _isPlayerTurn = !_isPlayerTurn;
        OnTurnChanged?.Invoke(this, new OnTurnChangedEventArgs { isPlayerTurn = _isPlayerTurn });
    }
}