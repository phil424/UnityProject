/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using UnityEngine.Profiling;

namespace JGT_Tools.PerfSight.Stats
{
    public class MemoryStat : IStatProvider
    {
        public string Id => "memory";
        public string Label => "MEM";
        public float Value { get; private set; }
        public string FormattedValue => Value.ToString("F1");
        public float OptimalValue { get; private set; }
        public float WarningValue { get; private set; }
        public string Unit => "GB";
        public string ValueColour => "#B388FF";
        public bool HigherIsBetter => false;

        public MemoryStat(float optimalValue, float warningValue)
        {
            OptimalValue = optimalValue;
            WarningValue = warningValue;
        }

        public void CalculateStat()
        {
            Value = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f * 1024f);
        }
    }
}
