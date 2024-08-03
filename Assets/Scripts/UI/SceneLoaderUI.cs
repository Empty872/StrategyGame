using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoaderUI : MonoBehaviour
{
    private Button _button;
    [SerializeField] private Loader.Scene _scene;
    public static Action<Loader.Scene> OnAnySceneLoaded;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            Loader.Load(_scene);
            OnAnySceneLoaded?.Invoke(_scene);
        });
        // OnAnySceneLoaded?.Invoke(_scene);
    }
}