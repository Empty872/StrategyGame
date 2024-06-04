using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffPanelUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _name;
    private Buff _buff;
    

    public void SetBuff(Buff buff)
    {
        _buff = buff;
        _name.text = buff.Name;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        BuffDescriptionUI.Instance.Show(_buff);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BuffDescriptionUI.Instance.Hide();
    }
}