using UnityEngine;
namespace JW.DungeonSliding
{
    public class GameUtil : MonoBehaviour
    {
        public static EDirectionType ReverseDirection(EDirectionType directionType)
        {
            int reverse = (int)directionType + 2;
            reverse = reverse % 4;

            return (EDirectionType)reverse;
        }
    }
}