using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private GameObject _blockPanel;
    [SerializeField] private TextMeshProUGUI _currentCooldownText;
    
    private Button _button;
    [SerializeField] private GameObject _selectedGO;
    private BaseAction _action;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetAction(BaseAction action)
    {
        if (!TurnSystem.Instance.IsPlayerTurn) return;
        _action = action;
        _name.text = action.GetName();
        _button.onClick.AddListener(() => { UnitActionSystem.Instance.SelectAction(action); });
    }
    


    public void UpdateVisual()
    {
        if (!_action.CanBeUsed()) ShowBlockPanel();
        _selectedGO.SetActive(_action == UnitActionSystem.Instance.SelectedAction);
        _button.onClick.AddListener(TrySelect);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillDescriptionUI.Instance.Show(_action);
    }

    public void ShowBlockPanel()
    {
        _blockPanel.SetActive(true);
        _currentCooldownText.text = _action.CurrentCooldown.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillDescriptionUI.Instance.Hide();
    }

    private void TrySelect()
    {
        if (!_action.CanBeUsed()) return;
        UnitActionSystem.Instance.SelectAction(_action);
    }
}