using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Map
{

    public enum EObstacleObjectType
    {
        None = -1,
        Rubble,
    }



    public interface IObstacleRequest
    {
        public void SpawnObstacle(Tile tile, EObstacleObjectType obsType);
        public void SpawnRanObstacle(Tile tile);
        public void ClearObstacles();
    }

    public class ObstacleObjectController : MonoBehaviour, IObstacleRequest
    {
        [SerializeField] private ObstacleObject[] _obsObjects;

        private IBoard _board;
        private Dictionary<EObstacleObjectType, Stack<ObstacleObject>> _obsObjPoolByType = new Dictionary<EObstacleObjectType, Stack<ObstacleObject>>();
        private List<ObstacleObject> _activeObsList = new List<ObstacleObject>();

        public void Wire(IBoard board)
        {
            _board = board;
        }

        public void SpawnObstacle(Tile tile, EObstacleObjectType obsType)
        {
            if (obsType == EObstacleObjectType.None) return;

            ObstacleObject obs = GetObs(obsType);
            obs.SetPosition(tile);
            obs.gameObject.SetActive(true);

            _activeObsList.Add(obs);

            _board.RegisterObstacleTile(tile);
        }
        public void SpawnRanObstacle(Tile tile)
        {

        }
        public ObstacleObject GetObs(EObstacleObjectType obstacleObjectType)
        {
            if(_obsObjPoolByType.TryGetValue(obstacleObjectType, out var value))
            {
                if(value.Count > 0)
                {
                    return value.Pop();
                }
                else
                {
                    return CreateObs(obstacleObjectType);
                }
            }
            else
            {
                _obsObjPoolByType.Add(obstacleObjectType, new Stack<ObstacleObject>());
                return CreateObs(obstacleObjectType);
            }
        }
        public ObstacleObject CreateObs(EObstacleObjectType obstacleObjectType)
        {
            ObstacleObject gameObject = Instantiate(_obsObjects[(int)obstacleObjectType]);
            return gameObject;
        }

        public void ClearObstacles()
        {
            for (int i = 0; i < _activeObsList.Count; i++)
            {
                ReturnObstacle(_activeObsList[i]);
            }

            _activeObsList.Clear();
        }
        private void ReturnObstacle(ObstacleObject obs)
        {
            obs.gameObject.SetActive(false);
            _obsObjPoolByType[obs.ObjectType].Push(obs);
            _board.UnRegisterObstacleTile(obs.TilePosition);
        }
    }
}
