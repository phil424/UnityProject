using System;
using System.Collections.Generic;

namespace MiniCrawler.Expedition
{
    public sealed class ActiveWorldEvent
    {
        public WorldEventDefinition Definition { get; }
        public WorldTimestamp StartedAt { get; }
        public WorldTimestamp EndsAt { get; private set; }

        public ActiveWorldEvent(WorldEventDefinition definition, WorldTimestamp startedAt, WorldTimestamp endsAt)
        {
            Definition = definition;
            StartedAt = startedAt;
            EndsAt = endsAt;
        }

        internal void ExtendTo(WorldTimestamp endsAt)
        {
            if (endsAt.CompareTo(EndsAt) > 0)
                EndsAt = endsAt;
        }
    }

    public sealed class WorldEventState
    {
        private readonly List<ActiveWorldEvent> activeEvents = new();

        public event Action Changed;
        public event Action<ActiveWorldEvent> Activated;
        public event Action<ActiveWorldEvent> Ended;

        public IReadOnlyList<ActiveWorldEvent> ActiveEvents => activeEvents;

        public bool IsActive(WorldEventDefinition definition)
        {
            return TryGetActive(definition, out _);
        }

        public bool TryGetActive(WorldEventDefinition definition, out ActiveWorldEvent activeEvent)
        {
            if (definition != null)
            {
                foreach (ActiveWorldEvent candidate in activeEvents)
                {
                    if (candidate.Definition != definition)
                        continue;

                    activeEvent = candidate;
                    return true;
                }
            }

            activeEvent = null;
            return false;
        }

        public bool TryActivate(
            WorldEventDefinition definition,
            WorldTimestamp startedAt,
            WorldTimestamp endsAt)
        {
            if (definition == null)
                return false;

            if (endsAt.CompareTo(startedAt) <= 0)
                endsAt = startedAt.AddMinutes(1d);

            if (TryGetActive(definition, out ActiveWorldEvent existing))
            {
                if (endsAt.CompareTo(existing.EndsAt) <= 0)
                    return false;

                existing.ExtendTo(endsAt);
                Changed?.Invoke();
                return true;
            }

            ActiveWorldEvent activeEvent = new(definition, startedAt, endsAt);

            activeEvents.Add(activeEvent);

            Activated?.Invoke(activeEvent);
            Changed?.Invoke();

            return true;
        }

        public int ExpireDue(WorldTimestamp currentTime)
        {
            int expiredCount = 0;

            for (int i = activeEvents.Count - 1; i >= 0; i--)
            {
                ActiveWorldEvent activeEvent = activeEvents[i];

                if (currentTime.CompareTo(activeEvent.EndsAt) < 0)
                    continue;

                activeEvents.RemoveAt(i);

                Ended?.Invoke(activeEvent);
                expiredCount++;
            }

            if (expiredCount > 0)
                Changed?.Invoke();

            return expiredCount;
        }
    }
}