using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform _arrowTransform;
    [SerializeField] private Transform _bowstringTransform;

    private Vector3 _startBowstringLocalPosition;

    // private UnitAnimator _unitAnimator;
    private bool _isActive;
    private float _timeToPullBowstring = 0;
    private float _timeToPullBowstringCurrent;
    

    private void Start()
    {
        _startBowstringLocalPosition = _bowstringTransform.localPosition;
        var arrowShotActions = GetComponents<ArrowShotAction>();
        foreach (var arrowShotAction in arrowShotActions)
        {
            arrowShotAction.OnShot += ArrowShotAction_OnShot;
            arrowShotAction.OnStartAiming += ArrowShotAction_OnStartAiming;
        }
    }

    private void ArrowShotAction_OnStartAiming(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        _isActive = true;
        _timeToPullBowstringCurrent = _timeToPullBowstring;
    }

    private void ArrowShotAction_OnShot(object sender, BaseAction.OnHostileBaseActionEventArgs e)
    {
        _bowstringTransform.localPosition = _startBowstringLocalPosition;
        _isActive = false;
    }

    private void Update()
    {
        if (!_isActive) return;
        if (_timeToPullBowstringCurrent > 0)
        {
            _timeToPullBowstring -= Time.deltaTime;
            return;
        }

        _bowstringTransform.position = _arrowTransform.position;
    }
}