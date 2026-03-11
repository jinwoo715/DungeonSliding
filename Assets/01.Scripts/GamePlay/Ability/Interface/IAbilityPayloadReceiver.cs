using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbilityPayloadReceiver<T>
    {
        void ReceivePayload(T payload);
    }
}
