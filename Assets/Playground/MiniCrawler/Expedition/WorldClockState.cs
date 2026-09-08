using System;

namespace MiniCrawler.Expedition
{
    public sealed class WorldClockState
    {
        private double totalWorldMinutes;

        public bool IsInitialized { get; private set; }

        public double ExpeditionElapsedSimulationSeconds { get; private set; }
        public double ExpeditionElapsedSimulationMinutes => ExpeditionElapsedSimulationSeconds / 60d;

        public WorldTimestamp StartTime { get; private set; }
        public WorldTimestamp CurrentTime => new(totalWorldMinutes);

        public void Initialize(double startingHour)
        {
            if (IsInitialized)
                return;

            double clampedStartingHour = Math.Max(0d, Math.Min(23.999d, startingHour));

            totalWorldMinutes = clampedStartingHour * WorldTimestamp.MinutesPerHour;
            StartTime = new WorldTimestamp(totalWorldMinutes);
            ExpeditionElapsedSimulationSeconds = 0d;
            IsInitialized = true;
        }

        public void Advance(double simulationDeltaSeconds, double worldHoursPerSimulationMinute)
        {
            if (!IsInitialized || simulationDeltaSeconds <= 0d)
                return;

            ExpeditionElapsedSimulationSeconds += simulationDeltaSeconds;

            if (worldHoursPerSimulationMinute <= 0d)
                return;

            // Numerically, 1 world hour / simulation minute is also
            // 1 world minute / simulation second.
            totalWorldMinutes += simulationDeltaSeconds * worldHoursPerSimulationMinute;
        }
    }
}