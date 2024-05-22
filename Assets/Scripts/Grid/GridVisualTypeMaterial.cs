using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct GridVisualTypeMaterial
{
    [SerializeField] private GridVisualType _gridVisualType;
    [SerializeField] private Material _material;
    public GridVisualType GridVisualType => _gridVisualType;
    public Material Material => _material;
}