using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitCharacteristic : MonoBehaviour
{
    public static event EventHandler OnAnyUnitCharacteristicChanged;
    // Start is called before the first frame update
    [SerializeField] private int _attack = 5;
    [SerializeField] private int _magicAttack = 5;
    [SerializeField] private int _defense = 5;
    [SerializeField] private int _speed = 3;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _maxActionPoints = 1;
    private int _defenseExtra;
    private int _attackExtra;
    public int Attack => _attack + _attackExtra;
    public int MagicAttack => _magicAttack;
    public int Defense => _defense + _defenseExtra;
    public int Speed => _speed;
    public int MaxHealth => _maxHealth;
    public int MaxActionPoints => _maxActionPoints;

    public void ChangeCharacteristic(CharacteristicType characteristicType, int amount)
    {
        switch (characteristicType)
        {
            case CharacteristicType.Attack:
                _attackExtra += amount;
                break;
            case CharacteristicType.MagicAttack:
                _magicAttack += amount;
                break;
            case CharacteristicType.Defense:
                _defense += amount;
                break;
            case CharacteristicType.Speed:
                _speed += amount;
                break;
            default:
                throw new NotImplementedException();
        }
        OnAnyUnitCharacteristicChanged?.Invoke(this, EventArgs.Empty);
    }
}