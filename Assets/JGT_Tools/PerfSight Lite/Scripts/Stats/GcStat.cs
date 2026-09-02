/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using System;
using Unity.Profiling;

namespace JGT_Tools.PerfSight.Stats
{
    public class GcStat : IStatProvider, IDisposable
    {
        public string Id => "gc";
        public string Label => "GC ALLOC";
        public float Value { get; private set; }
        public string FormattedValue => Value.ToString("F0");
        public float OptimalValue { get; private set; }
        public float WarningValue { get; private set; }
        public string Unit => "B";
        public string ValueColour => "#FFD93D";
        public bool HigherIsBetter => false;

        private ProfilerRecorder _profilerRecorder;

        public GcStat(float optimalValue, float warningValue)
        {
            _profilerRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");

            OptimalValue = optimalValue;
            WarningValue = warningValue;
        }

        public void CalculateStat()
        {
            if (!_profilerRecorder.Valid || _profilerRecorder.Count <= 0)
                return;

            Value = _profilerRecorder.LastValue;
        }

        public void Dispose()
        {
            if (_profilerRecorder.Valid)
                _profilerRecorder.Dispose();
        }
    }
}
