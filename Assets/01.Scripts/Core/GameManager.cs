using UnityEngine;
using JW.DungeonSliding.Core.Resource;

namespace JW.DungeonSliding.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        public ResourceManager Resource { get; private set; }

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
            Resource = GetComponentInChildren<ResourceManager>();
            Resource.Init();
        }
    }
}
