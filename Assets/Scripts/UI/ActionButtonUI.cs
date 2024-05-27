using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _name;
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
        _button.onClick.AddListener(() => { UnitActionSystem.Instance.SelectAction(action); });
    }

    public void UpdateVisual()
    {
        _selectedGO.SetActive(_action == UnitActionSystem.Instance.SelectedAction);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerInside = true;
        SkillDescriptionUI.Instance.Show(_action);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
        // Start a coroutine to delay the hiding in case the pointer enters another child element immediately
        StartCoroutine(CheckPointerExit());
    }

    private IEnumerator CheckPointerExit()
    {
        yield return new WaitForEndOfFrame();
        if (!_isPointerInside)
        {
            SkillDescriptionUI.Instance.Hide();
        }
    }
}