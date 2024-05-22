using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMeshPro;
    private object _gridObject;

    void Update()
    {
        SetText();
    }

    public virtual void SetGridObject(object gridObject)
    {
        _gridObject = gridObject;
    }

    protected virtual void SetText()
    {
        _textMeshPro.text = _gridObject.ToString();
    }
}