using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _actionPointsText;
    [SerializeField] private TextMeshProUGUI _cooldownText;

    public static SkillDescriptionUI Instance { get; private set; }

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

    private void UpdateDescription(BaseAction action)
    {
        _descriptionText.text = action.GetDescription();
        _actionPointsText.text = "AP: " + action.GetActionPointsCost();
        _cooldownText.text = "CD: " + action.GetCooldown();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(BaseAction action)
    {
        UpdateDescription(action);
        gameObject.SetActive(true);
    }
}