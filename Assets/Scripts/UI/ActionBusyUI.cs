using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        Hide();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool e)
    {
        if (e) Show();
        else Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}