using System;
using UnityEngine;

namespace GameData
{
    public class StartState : MonoBehaviour
    {
        [SerializeField] private Loader.Scene _scene;
        //
        // public void SendStartGame()
        // {
        //     Application.ExternalCall("ReceiveStartGame");
        // }
        //
        public void StartGame()
        {
            Loader.Load(_scene);
            SendLoadGameScene();
        }

        public static StartState Instance;

        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            SceneLoaderUI.OnAnySceneLoaded += scene =>
            {
                switch (scene)
                {
                    case Loader.Scene.MenuScene:
                        SendLoadMenuScene();
                        break;
                    case Loader.Scene.GameScene:
                        SendLoadGameScene();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(scene), scene, null);
                }
            };
        }
        private void SendLoadMenuScene() => Application.ExternalCall("ReceiveLoadMenuScene");
        private void SendLoadGameScene() => Application.ExternalCall("ReceiveLoadGameScene");
    }
}