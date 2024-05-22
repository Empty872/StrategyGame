using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Show(Material material)
    {
        _meshRenderer.enabled = true;
        _meshRenderer.material = material;
    }

    public void Hide()
    {
        _meshRenderer.enabled = false;
    }
}