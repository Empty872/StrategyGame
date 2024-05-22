using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    private float _movementSpeed = 10;
    private float _rotationSpeed = 100;
    private float _zoomSpeed = 1;
    private float _zoomAnimationSpeed = 5;
    private Vector3 _targetFollowOffset;
    private float _minTargetFollowOffsetY = 2;
    private float _maxTargetFollowOffsetY = 12;
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private CinemachineTransposer _cinemachineTransposer;


    private void Start()
    {
        _cinemachineTransposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _targetFollowOffset = _cinemachineTransposer.m_FollowOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        var inputMovementDir = InputManager.Instance.GetCameraMoveVector();
        var movementVector = transform.forward * inputMovementDir.y + transform.right * inputMovementDir.x;
        transform.position += movementVector * _movementSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        var inputRotationVector = Vector3.zero;
        inputRotationVector.y = InputManager.Instance.GetCameraRotateAmount();
        transform.eulerAngles += inputRotationVector * _rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        _targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount();
        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, _minTargetFollowOffsetY, _maxTargetFollowOffsetY);
        _cinemachineTransposer.m_FollowOffset =
            Vector3.Lerp(_cinemachineTransposer.m_FollowOffset, _targetFollowOffset,
                Time.deltaTime * _zoomAnimationSpeed);
    }
}