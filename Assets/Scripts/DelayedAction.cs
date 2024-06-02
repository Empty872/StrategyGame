using System;
using UnityEngine;

public class DelayedAction
{
    private int _turnsToDisappearing;
    private Action _onEndAction;
    private bool _isPlayer;

    public DelayedAction(Action onEndAction, int turnsToDisappearing, bool isPlayer)
    {
        _turnsToDisappearing = turnsToDisappearing;
        _onEndAction = onEndAction;
        _isPlayer = isPlayer;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        if (e.isPlayerTurn == _isPlayer)
        {
            ReduceDelay();
        }
    }

    public void ReduceDelay()
    {
        _turnsToDisappearing -= 1;
        if (_turnsToDisappearing == 0) End();
    }

    public void End()
    {
        _onEndAction?.Invoke();
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}