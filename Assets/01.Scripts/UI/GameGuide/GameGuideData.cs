using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public enum EGuideType
    {
        Move,
        Battle,
        Enemy,
        EffectTile,
        Ability
    }


    [System.Serializable]
    public class GameGuideData
    {
        public Sprite _sprite;

        [TextArea]
        public string _description;
    }


}
