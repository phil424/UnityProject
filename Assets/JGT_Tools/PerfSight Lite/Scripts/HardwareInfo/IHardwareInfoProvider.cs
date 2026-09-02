/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

namespace JGT_Tools.PerfSight.HardwareInfo
{
    public interface IHardwareInfoProvider
    {
        string Id { get; }
        string Label { get; }
        string Value { get; }

        void GetHardwareInfo();
    }
}
