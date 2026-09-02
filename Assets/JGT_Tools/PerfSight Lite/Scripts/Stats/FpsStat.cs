/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using UnityEngine;

namespace JGT_Tools.PerfSight.Stats
{
    public class FpsStat : IStatProvider
    {
        public string Id => "fps";
        public string Label => "FPS";
        public float Value { get; private set; }
        public string FormattedValue => Value.ToString("F0");
        public float OptimalValue {get; private set; }
        public float WarningValue {get; private set; }
        public string Unit => "FPS";
        public string ValueColour => "#39FF88";
        public bool HigherIsBetter => true;

        public FpsStat(float optiomalValue, float warningValue)
        {
            OptimalValue = optiomalValue;
            WarningValue = warningValue;
        }

        public void CalculateStat()
        {
            Value = 1f / Time.unscaledDeltaTime;
        }
    }
}
