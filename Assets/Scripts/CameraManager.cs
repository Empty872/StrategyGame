using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject _actionCamera;

    private void Start()
    {
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        HideActionCamera();
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                HideActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        switch (sender)
        {
            case ShootAction shootAction:
                var shooterUnitPosition = shootAction.Unit.WorldPosition;
                var targetUnitPosition = shootAction.TargetUnit.WorldPosition;
                var yOffsetVector = Vector3.up * 1.7f;
                var shootDir = (targetUnitPosition - shooterUnitPosition).normalized;
                var xOffset = 0.5f;
                var zOffset = -1f;
                var xOffsetVector = Quaternion.Euler(0, 90, 0) * shootDir * xOffset;
                var zOffsetVector = shootDir * zOffset;
                var actionCameraPosition = shooterUnitPosition + yOffsetVector + xOffsetVector + zOffsetVector;
                _actionCamera.transform.position = actionCameraPosition;
                _actionCamera.transform.LookAt(targetUnitPosition + yOffsetVector);
                ShowActionCamera();
                break;
        }
    }

    // Start is called before the first frame update
    private void ShowActionCamera()
    {
        _actionCamera.SetActive(true);
    }

    private void HideActionCamera()
    {
        _actionCamera.SetActive(false);
    }
}