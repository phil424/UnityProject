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
    public class TrianglesStat : IStatProvider, IDisposable
    {
        public string Id => "triangles";
        public string Label => "TRIS";
        public float Value { get; private set; }
        public string FormattedValue => Value.ToString("F0");
        public float OptimalValue {get; private set; }
        public float WarningValue {get; private set; }
        public string Unit => string.Empty;
        public string ValueColour => "#FFFFFF";
        public bool HigherIsBetter => false;

        private ProfilerRecorder _profilerRecorder;

        private long _lastValidValue;
        private bool _hasValidValue;

        public TrianglesStat(float optimalValue, float warningValue)
        {
            _profilerRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");

            OptimalValue = optimalValue;
            WarningValue = warningValue;
        }

        public void CalculateStat()
        {
            if (!_profilerRecorder.Valid || _profilerRecorder.Count <= 0)
                return;

            long currentValue = _profilerRecorder.LastValue;

            if (currentValue == 0 && _hasValidValue)
            {
                Value = _lastValidValue;
                return;
            }

            _lastValidValue = currentValue;
            _hasValidValue = true;
            Value = currentValue;
        }

        public void Dispose()
        {
            if (_profilerRecorder.Valid)
                _profilerRecorder.Dispose();
        }
    }
}
