using UnityEngine;
using JW.DungeonSliding.Core.Resource;
using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.Core.Data;

namespace JW.DungeonSliding.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [SerializeField] private ResourceManager _resource;

        public static DataManager Data { get; } = new DataManager();
        public static ResourceManager Resource => _instance._resource;
        public static GameConfig Config => Resource.GameConfig;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // 하위 매니저 초기화
            InitManagers();
        }
        private void InitManagers()
        {
            Resource.Init();
            Data.Initialize();
        }
    }
}
