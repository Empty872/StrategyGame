using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private GameObject _blockPanel;
    [SerializeField] private TextMeshProUGUI _currentCooldownText;
    private Button _button;
    [SerializeField] private GameObject _selectedGO;
    private BaseAction _action;
    private bool _isPointerInside = false;
    public BaseAction Action => _action;


    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetAction(BaseAction action)
    {
        _action = action;
        _name.text = action.GetName();
        _button.onClick.AddListener(TrySelect);
        _action.OnActionStarted += Action_OnActionStarted;
    }

    private void Action_OnActionStarted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (!_action.CanBeUsed()) ShowBlockPanel();
        _selectedGO.SetActive(_action == UnitActionSystem.Instance.SelectedAction && _action.CanBeUsed());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerInside = true;
        SkillDescriptionUI.Instance.Show(_action);
    }

    public void ShowBlockPanel()
    {
        _blockPanel.SetActive(true);
        _currentCooldownText.text = _action.CurrentCooldown.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
        SkillDescriptionUI.Instance.Hide();
    }

    private void TrySelect()
    {
        if (!_action.CanBeUsed()) return;
        UnitActionSystem.Instance.SelectAction(_action);
    }
}