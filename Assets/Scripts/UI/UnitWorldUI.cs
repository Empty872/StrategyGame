using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionPointsText;
    [SerializeField] private Image _healthBarFiller;
    [SerializeField] private Unit _unit;
    [SerializeField] private HealthSystem _healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        _healthSystem.OnTakeDamage += HealthSystem_OnDamaged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void UpdateActionPointsText()
    {
        _actionPointsText.text = _unit.ActionPoints.ToString();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void UpdateHealthBar()
    {
        _healthBarFiller.fillAmount = _healthSystem.GetHealthNormalized();
    }

    private void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
}