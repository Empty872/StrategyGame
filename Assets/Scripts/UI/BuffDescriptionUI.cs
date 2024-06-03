using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class BuffDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _remainTurnsText;

    public static BuffDescriptionUI Instance { get; private set; }

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
        Hide();
    }

    private void UpdateDescription(Buff buff)
    {
        _descriptionText.text = buff.Description;
        _remainTurnsText.text = "Turns remain " + buff.CurrentCooldown;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(Buff buff)
    {
        UpdateDescription(buff);
        gameObject.SetActive(true);
    }
}