using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler OnDeath;
    public event EventHandler OnHealthChanged;
    public static event EventHandler OnAnyHealthChanged;

    private int _health;
    public int MaxHealth => _unitCharacteristic.MaxHealth;
    public int Health => _health;
    private UnitCharacteristic _unitCharacteristic;

    // Start is called before the first frame update
    private void Awake()
    {
        _unitCharacteristic = GetComponent<UnitCharacteristic>();
    }

    private void Start()
    {
        _health = MaxHealth;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        OnAnyHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        OnAnyHealthChanged?.Invoke(this, EventArgs.Empty);

        if (_health <= 0) Die();
    }

    public void RestoreHealth(int count)
    {
        _health += count;
        _health = Mathf.Min(_health, MaxHealth);
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
        OnAnyHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() => (float)_health / _unitCharacteristic.MaxHealth;
}