using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public class GameTriggerEventBus
    {
        public static GameTriggerEventBus Instance { get; private set; }

        public EGameEventTrigger Trigger;

        public Dictionary<EGameEventTrigger, Action> _triggerEventsByTriggerType = new Dictionary<EGameEventTrigger, Action>();
        public Dictionary<EGameEventTrigger, Queue<Action>> _instanceEventQueue = new Dictionary<EGameEventTrigger, Queue<Action>>();
        public GameTriggerEventBus()
        {
            Instance = this;
        }
        public void ClearInstance()
        {
            if (Instance == this) Instance = null;
            _triggerEventsByTriggerType.Clear(); // 데이터도 함께 청소
        }
        public void SubscribeTriggerEvent(EGameEventTrigger triggerType, Action bindAction)
        {
            if (!_triggerEventsByTriggerType.ContainsKey(triggerType))
            {
                _triggerEventsByTriggerType[triggerType] = delegate { };
            }

            _triggerEventsByTriggerType[triggerType] += bindAction;
        }
        public void UnSubscribeTriggerEvent(EGameEventTrigger triggerType, Action bindAction)
        {
            _triggerEventsByTriggerType[triggerType] -= bindAction;
        }

        public void EnqueueInstanceTriggerEvent(EGameEventTrigger trigger, Action instanceAction)
        {
            if (!_instanceEventQueue.ContainsKey(trigger))
            {
                _instanceEventQueue.Add(trigger, new Queue<Action>());
            }

            _instanceEventQueue[trigger].Enqueue(instanceAction);
        }

        public void ExcuteAbilityEvent(EGameEventTrigger triggerType)
        {
            if(_triggerEventsByTriggerType.TryGetValue(triggerType, out Action action))
            {
                action?.Invoke();
            }

            if(_instanceEventQueue.TryGetValue(triggerType, out var queue))
            {
                while(queue.Count != 0)
                {
                    var instanceEvent = queue.Dequeue();
                    instanceEvent?.Invoke();
                }
            }
        }
    }
}