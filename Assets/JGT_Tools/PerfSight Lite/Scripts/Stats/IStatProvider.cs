/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

namespace JGT_Tools.PerfSight.Stats
{
    public interface IStatProvider
    {
        string Id { get; }
        string Label { get; }
        float Value { get; }
        string FormattedValue { get; }
        float OptimalValue { get; }
        float WarningValue { get; }
        string Unit { get; }
        string ValueColour { get; }

        bool HigherIsBetter => true;

        void CalculateStat();
    }
}
