using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoaderUI : MonoBehaviour
{
    private Button _button;
    [SerializeField] private Loader.Scene _scene;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => { Loader.Load(_scene); });
    }
}