using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [DefaultExecutionOrder(-500)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        [HideInInspector]
        public static GlobalGameSettings globalGameSettings;

        [SerializeField]
        private GlobalGameSettings g_gameSettings;

        public PlayerRoot localPlayer;
        public InGameGUI inGameGUI;

        private GameObject inGameGUIPrefab;
        private GameObject decalManagerPrefab;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            globalGameSettings = g_gameSettings;

            decalManagerPrefab = Resources.Load<GameObject>("Prefabs/Managers/Decal Manager");
            inGameGUIPrefab = Resources.Load<GameObject>("Prefabs/UI/In Game GUI");
        }

        public void OnLoadMap()
        {
            if (decalManagerPrefab)
            {
                Instantiate(decalManagerPrefab);
            }

            if (inGameGUIPrefab)
            {
                inGameGUI = Instantiate(inGameGUIPrefab).GetComponentInChildren<InGameGUI>();
            }
        }
    }
}