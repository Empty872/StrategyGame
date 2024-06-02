using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInputActions _playerInputActions;


    private void Awake()
    {
        if (Instance is not null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
    }


    public Vector2 GetMouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    public bool IsLeftMouseButtonDownThisFrame()
    {
        return _playerInputActions.Player.LeftClick.WasPerformedThisFrame();
    }
    public bool IsRightMouseButtonDownThisFrame()
    {
        return _playerInputActions.Player.RightClick.WasPerformedThisFrame();
    }

    public Vector2 GetCameraMoveVector()
    {
        return _playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
    }

    public float GetCameraRotateAmount()
    {
        return _playerInputActions.Player.CameraRotate.ReadValue<float>();
    }

    public float GetCameraZoomAmount()
    {
        return _playerInputActions.Player.CameraZoom.ReadValue<float>();
    }

}
