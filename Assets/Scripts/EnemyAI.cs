using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Start is called before the first frame update
    private float _timer = 2;
    private State _state;
    public static EnemyAI Instance { get; private set; }

    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _state = State.WaitingForEnemyTurn;
    }

    void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, TurnSystem.OnTurnChangedEventArgs e)
    {
        if (e.isPlayerTurn) return;
        _state = State.TakingTurn;
        _timer = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn) return;
        switch (_state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _state = State.Busy;
                    if (TryTakeEnemyAIAction(SetStateTakingTurn)) _state = State.Busy;
                    else TurnSystem.Instance.NextTurn();
                }

                break;
            case State.Busy:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIAActionComplete)
    {
        foreach (var enemyUnit in UnitManager.Instance.EnemyUnitList)
        {
            UnitActionSystem.Instance.SelectUnit(enemyUnit, true);
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIAActionComplete)) return true;
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIAActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestAction = null;
        ;
        foreach (var action in enemyUnit.ActionArray)
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(action)) continue;
            if (bestEnemyAIAction is null)
            {
                bestEnemyAIAction = action.GetBestEnemyAIAction();
                bestAction = action;
            }
            else
            {
                var possibleEnemyAIAction = action.GetBestEnemyAIAction();
                if (possibleEnemyAIAction is not null &&
                    possibleEnemyAIAction.actionPriority > bestEnemyAIAction.actionPriority)
                {
                    bestEnemyAIAction =possibleEnemyAIAction;
                    bestAction = action;
                }
            }
        }

        if (bestEnemyAIAction is not null && enemyUnit.TrySpendActionPointsToTakeAction(bestAction))
        {
            bestAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIAActionComplete);
            return true;
        }

        return false;
    }

    private void SetStateTakingTurn()
    {
        _timer = 0.5f;
        _state = State.TakingTurn;
    }
}