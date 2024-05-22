using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshProUGUI _name;
    private Button _button;
    [SerializeField] private GameObject _selectedGO;
    private BaseAction _action;

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
}