using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitUI : MonoBehaviour
{
    [SerializeField] private Image _healthBarFiller;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _magicAttackText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private Transform _buffPanelContainer;
    [SerializeField] private Transform _buffPanelPrefab;
    private UnitActionSystem _unitActionSystem;

    private void Start()
    {
        _unitActionSystem = UnitActionSystem.Instance;
        _unitActionSystem.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitCharacteristic.OnAnyUnitCharacteristicChanged += UnitCharacteristic_OnAnyUnitCharacteristicChanged;
        HealthSystem.OnAnyHealthChanged += HealthSystem_OnAnyHealthChanged;
        BuffSystem.OnAnyBuffListChanged += BuffSystem_OnAnyBuffListChanged;
        UpdateHealth();
        UpdateCharacteristic();
        UpdateBuffs();
    }

    private void BuffSystem_OnAnyBuffListChanged(object sender, EventArgs e)
    {
        UpdateBuffs();
    }

    private void HealthSystem_OnAnyHealthChanged(object sender, EventArgs e)
    {
        UpdateHealth();
    }

    private void UnitCharacteristic_OnAnyUnitCharacteristicChanged(object sender, EventArgs e)
    {
        UpdateCharacteristic();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateHealth();
        UpdateCharacteristic();
        UpdateBuffs();
    }


    private void UpdateHealth()
    {
        var selectedUnit = _unitActionSystem.SelectedUnit;
        _healthText.text = selectedUnit.Health + "/" + selectedUnit.MaxHealth;
        _healthBarFiller.fillAmount = selectedUnit.GetHealthNormalized();
    }

    private void UpdateCharacteristic()
    {
        var selectedUnit = _unitActionSystem.SelectedUnit;
        _attackText.text = "ATK" + "\n" + selectedUnit.Attack;
        _defenseText.text = "DEF" + "\n" + selectedUnit.Defense;
        _magicAttackText.text = "MAG" + "\n" + selectedUnit.MagicAttack;
        _speedText.text = "SPD" + "\n" + selectedUnit.Speed;
    }

    private void UpdateBuffs()
    {
        foreach (Transform buffPanel in _buffPanelContainer)
        {
            Destroy(buffPanel.gameObject);
        }

        var selectedUnit = _unitActionSystem.SelectedUnit;
        var buffList = selectedUnit.BuffList;
        foreach (var buff in buffList)
        {
            var buffPanelTransform = Instantiate(_buffPanelPrefab, _buffPanelContainer);
            var buffPanelUI = buffPanelTransform.GetComponent<BuffPanelUI>();
            buffPanelUI.SetBuff(buff);
        }
    }
}