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
    public class FrameTimeStat : IStatProvider
    {
        public string Id => "frametime";
        public string Label => "FRAME TIME";
        public float Value { get; private set; }
        public string FormattedValue => Value.ToString("F1");
        public float OptimalValue {get; private set; }
        public float WarningValue {get; private set; }
        public string Unit => "MS";
        public string ValueColour => "#7CFF6B";
        public bool HigherIsBetter => false;

        public FrameTimeStat(float optimalValue, float warningValue)
        {
            OptimalValue = optimalValue;
            WarningValue = warningValue;
        }

        public void CalculateStat()
        {
            // Milliseconds per frame
            Value = Time.deltaTime * 1000f;
        }
    }
}
