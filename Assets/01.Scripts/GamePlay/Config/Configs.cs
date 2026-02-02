using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public class Configs
    {
        private GameConfig _gameConfig;
        public GameConfig GameConfig => _gameConfig;

        public void Init(GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
        }
    }
}
