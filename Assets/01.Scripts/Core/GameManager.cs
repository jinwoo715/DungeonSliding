using UnityEngine;
using JW.DungeonSliding.Core.Resource;
using JW.DungeonSliding.GamePlay;

namespace JW.DungeonSliding.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [SerializeField] private ResourceManager _resource;
        public ResourceManager Resource => _resource;

        public static Configs _configs = new Configs();
        public static Configs Configs => _configs;

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
            _configs.Init(Resource.GameConfig);
        }
    }
}
