using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridVisualManager: MonoBehaviour
{
    [SerializeField] private List<GridColorMaterial> _gridColorMaterialList = new ();
    private Dictionary<GridColorEnum, GridColorMaterial> _colorMaterialDictionary = new ();
    public static GridVisualManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        foreach (var gridColorMaterial in _gridColorMaterialList)
        {
            _colorMaterialDictionary.Add(gridColorMaterial.Color, gridColorMaterial);
        }
    }

    public Material GetReachableColor(BaseAction action)
    {
        return action switch
        {
            MoveAction => _colorMaterialDictionary[GridColorEnum.Blue].Material,
            SpinAction => _colorMaterialDictionary[GridColorEnum.Blue].Material,
            InteractAction => _colorMaterialDictionary[GridColorEnum.Green].Material,
            _ => _colorMaterialDictionary[GridColorEnum.Red].Material
        };
    }

    public Material GetPossibleColor(BaseAction action)
    {
        return action switch
        {
            MoveAction => _colorMaterialDictionary[GridColorEnum.Blue].HighlightedMaterial,
            SpinAction => _colorMaterialDictionary[GridColorEnum.Blue].HighlightedMaterial,
            InteractAction => _colorMaterialDictionary[GridColorEnum.Green].HighlightedMaterial,
            _ => _colorMaterialDictionary[GridColorEnum.Red].HighlightedMaterial
        };
    }
}