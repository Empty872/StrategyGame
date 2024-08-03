using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class GameState : MonoBehaviour
{
    private charactersContainer playerCharacters;
    private charactersContainer opponentCharacters;
    private actionsContainer actions;
    public static GameState Instance;

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
        actions.actions = new(10);
        BaseAction.OnAnyActionStartedDescription += (unit, actionName) =>
        {
            actions.actions.Enqueue(GetActionDescription(unit, actionName));
            if (actions.actions.Count == 10) actions.actions.Dequeue();
            SendActionsData();
        };
        TurnSystem.Instance.OnTurnChanged += (sender, args) => SendTurnData();
        GameManager.Instance.OnWin += SendWin;
        GameManager.Instance.OnDefeat += SendDefeat;
        SceneLoaderUI.OnAnySceneLoaded += scene =>
        {
            switch (scene)
            {
                case Loader.Scene.MenuScene:
                    SendLoadMenuScene();
                    break;
                case Loader.Scene.GameScene:
                    SendLoadGameScene();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
            }
        };
    }

    private void Update()
    {
        // SendTurnData();
        SendCharactersData();
        // SendActionsData();
    }

    private void SetCharacters()
    {
        playerCharacters.characters = UnitManager.Instance.FriendlyUnitList.Select(CharacterStateTest.Create).ToList();
        opponentCharacters.characters = UnitManager.Instance.EnemyUnitList.Select(CharacterStateTest.Create).ToList();
    }

    public void SendTurnData()
    {
        Application.ExternalCall("ReceiveTurnData", TurnSystem.Instance.TurnNumber,
            TurnSystem.Instance.IsPlayerTurn ? 0 : 1);
    }

    public void SendCharactersData()
    {
        SetCharacters();
        var playerData = JsonUtility.ToJson(playerCharacters, true);
        var opponentData = JsonUtility.ToJson(opponentCharacters, true);
        Application.ExternalCall("ReceiveCharactersData", playerData, opponentData);
    }

    public void SendActionsData()
    {
        Application.ExternalCall("ReceiveActionsData", actions.actions.ToList());
    }

    public void EndTurn()
    {
        TurnSystem.Instance.NextTurn();
    }

    private string GetActionDescription(Unit unit, string actionName)
    {
        return (unit.IsEnemy ? "Вражеский " : "Ваш ") + unit.Name + " использовал " + actionName;
    }

    private void SendWin() => Application.ExternalCall("ReceiveWin");
    private void SendDefeat() => Application.ExternalCall("ReceiveDefeat");
    private void SendLoadMenuScene() => Application.ExternalCall("ReceiveLoadMenuScene");
    private void SendLoadGameScene() => Application.ExternalCall("ReceiveLoadGameScene");
}

[Serializable]
public class CharacterStateTest
{
    public string Name;
    public int Health;
    public int MaxHealth;
    public int Attack;
    public int Defense;
    public int MagicAttack;
    public int Speed;
    public string Statuses;

    public static CharacterStateTest Create(Unit unit)
    {
        return new CharacterStateTest
        {
            Name = unit.Name,
            Health = unit.Health,
            MaxHealth = unit.MaxHealth,
            Attack = unit.Attack,
            Defense = unit.Defense,
            MagicAttack = unit.MagicAttack,
            Speed = unit.Speed,
            Statuses = string.Join(", ", unit.BuffList.Select(x => x.Name))
        };
    }
}

[Serializable]
public struct charactersContainer
{
    public List<CharacterStateTest> characters;
}

public struct actionsContainer
{
    public Queue<string> actions;
}