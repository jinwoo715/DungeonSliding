using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbilityPayloadReceiver<T>
    {
        void ReceivePayload(T payload);
    }
    public interface IAbilityPayloadSender
    {
        public void SendPayload<T>(T payload);
    }
}
