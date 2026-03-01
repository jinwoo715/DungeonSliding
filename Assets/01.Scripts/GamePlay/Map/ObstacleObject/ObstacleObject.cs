using JW.DungeonSliding.Map;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class ObstacleObject : MonoBehaviour, ITileObject
    {
        public EObstacleObjectType ObjectType;
        public ParticleSystem Particle;
        public Tile TilePosition { get; private set; }

        public void SetPosition(Tile point)
        {
            TilePosition = point;
            this.transform.position = point.GetPosition;
        }

        private void OnEnable()
        {
            Particle.Play();
        }
    }
}
