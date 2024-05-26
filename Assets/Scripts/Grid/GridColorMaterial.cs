using System;
using UnityEngine;

[Serializable]
public struct GridColorMaterial
{
    [SerializeField] private GridColorEnum _color;
    [SerializeField] private Material _materialHighlighted;
    [SerializeField] private Material _material;
    public GridColorEnum Color => _color;
    public Material HighlightedMaterial => _materialHighlighted;
    public Material Material => _material;

}