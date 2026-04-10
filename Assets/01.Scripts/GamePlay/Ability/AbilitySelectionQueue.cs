using System;
using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilitySelectionQueue
    {
        private readonly Queue<AbilitySelectSession> _sessions = new();
        public event Action<AbilitySelectSession> OnExecuteSession;

        public void Enqueue(AbilitySelectSession session)
        {
            _sessions.Enqueue(session);
        }

        public void Progress()
        {
            if (_sessions.Count > 0)
            {
                OnExecuteSession?.Invoke(_sessions.Dequeue());
            }
        }

        public int Count => _sessions.Count;
        public bool HasSessions => _sessions.Count > 0;

        public void Clear()
        {
            _sessions.Clear();
        }
    }
}
