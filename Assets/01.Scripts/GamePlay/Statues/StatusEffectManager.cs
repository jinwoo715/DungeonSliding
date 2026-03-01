using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Statues
{
    public class StatusEffectManager : IStatusModifier, IStatusReadOnly
    {
        private ECreatureStatus _statusFlags;
        private Dictionary<ECreatureStatus, int> _statusDurations = new();

        public bool HasStatus(ECreatureStatus status)
        {
            return status != ECreatureStatus.None && (_statusFlags & status) == status;
        }

        public void ApplyStatus(ECreatureStatus status, int durationTurnCount)
        {
            _statusFlags |= status;

            if (_statusDurations.TryGetValue(status, out int value))
            {
                if (value < durationTurnCount) _statusDurations[status] = durationTurnCount;
            }
            else
            {
                _statusDurations.Add(status, durationTurnCount);
            }
        }
        public void RemoveStatus(ECreatureStatus status)
        {
            _statusFlags &= ~status;

            if (_statusDurations.ContainsKey(status))
            {
                _statusDurations.Remove(status);
            }
        }

        public void TimePassStatueUpdate()
        {
            var keys = _statusDurations.Keys.ToList();

            foreach (var key in keys)
            {
                // duration이 -1(영구 지속)인 상태(예: 배리어)는 턴 계산을 건너뜀
                if (_statusDurations[key] == -1) continue;

                int remainDuration = _statusDurations[key]--;

                if (remainDuration <= 0)
                {
                    RemoveStatus(key);
                }
            }
        }

        public void ClearAllStatus()
        {
            _statusDurations.Clear();
            _statusFlags = ECreatureStatus.None;
        }


    }
}
