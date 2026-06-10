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
        [SerializeField] private SoundManager _soundManager;
        
        private SceneManagerEx _scene;

        public static SceneManagerEx Scene => _instance._scene;
        public static DataManager Data { get; } = new DataManager();
        public static ResourceManager Resource => _instance._resource;
        public static GameConfig Config => Resource.GameConfig;
        public static ISound Sound => _instance._soundManager;

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
            _scene = new SceneManagerEx();
            _soundManager.Init(_resource.Clips);
        }
    }
}
