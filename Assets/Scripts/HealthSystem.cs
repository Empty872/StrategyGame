using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 10;
    public event EventHandler OnDeath;
    public event EventHandler OnTakeDamage;

    private float _health;

    // Start is called before the first frame update
    private void Awake()
    {
        _health = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        OnTakeDamage?.Invoke(this, EventArgs.Empty);
        if (_health <= 0) Die();
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() => _health / _maxHealth;
}