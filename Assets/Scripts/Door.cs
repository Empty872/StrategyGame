using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;

    private GridPosition _gridPosition;
    private Animator _animator;
    private Action _onInteractCompleteAction;
    private bool _isActive;
    private float _timer;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this);

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _isActive = false;
            _onInteractCompleteAction();
        }
    }


    public void Interact(Action onInteractCompleteAction)
    {
        _onInteractCompleteAction = onInteractCompleteAction;
        _isActive = true;
        _timer = .5f;

        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        _animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        _animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(_gridPosition, false);
    }
}