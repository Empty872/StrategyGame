using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseWorld : MonoBehaviour
{
    public static MouseWorld Instance { get; private set; }
    [FormerlySerializedAs("mousePlaneLayer")] [SerializeField] private LayerMask _mousePlaneLayer;
    public LayerMask MousePlaneMask => _mousePlaneLayer;

    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        transform.position = GetPosition();
    }

    public static Vector3 GetPosition()
    {
        var ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance._mousePlaneLayer);
        return raycastHit.point;
    }
}