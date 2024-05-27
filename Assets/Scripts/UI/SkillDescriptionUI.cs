using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillDescriptionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _descriptionText;

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

    private void UpdateDescription(BaseAction action)
    {
        _descriptionText.text = action.GetDescription();
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