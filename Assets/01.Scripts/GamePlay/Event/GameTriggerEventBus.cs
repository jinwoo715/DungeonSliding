using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public class GameTriggerEventBus
    {
        public static GameTriggerEventBus Instance { get; private set; }

        public EGameTriggerType Trigger;

        public Dictionary<EGameTriggerType, Action> _triggerEventsByTriggerType = new Dictionary<EGameTriggerType, Action>();

        public GameTriggerEventBus()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log($"{Instance}");                                                                                             
                Debug.LogWarning("이미 존재하는 GameTriggerEventBus 인스턴스가 있습니다. 기존 인스턴스를 덮어씁니다.");
            }
            Instance = this;
        }

        public void ClearInstance()
        {
            if (Instance == this) Instance = null;
            _triggerEventsByTriggerType.Clear(); // 데이터도 함께 청소
        }
        public void SubscribeTriggerEvent(EGameTriggerType triggerType, Action bindAction)
        {
            if (!_triggerEventsByTriggerType.ContainsKey(triggerType))
            {
                _triggerEventsByTriggerType[triggerType] = delegate { };
            }

            _triggerEventsByTriggerType[triggerType] += bindAction;
        }
        public void UnSubscribeTriggerEvent(EGameTriggerType triggerType, Action bindAction)
        {
            _triggerEventsByTriggerType[triggerType] -= bindAction;
        }
        public void ExcuteAbilityEvent(EGameTriggerType triggerType)
        {
            Debug.Log(triggerType);
            if(_triggerEventsByTriggerType.TryGetValue(triggerType, out Action action))
            {
                action?.Invoke();
            }
        }
    }
}