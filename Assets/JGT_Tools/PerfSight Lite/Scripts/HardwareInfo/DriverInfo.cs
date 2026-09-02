/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using UnityEngine;
using JGT_Tools.PerfSight.Helpers;

namespace JGT_Tools.PerfSight.HardwareInfo
{
    public class DriverInfo : IHardwareInfoProvider
    {
        public string Id => "driver";
        public string Label => "DRIVER";
        public string Value { get; private set; }

        public void GetHardwareInfo()
        {
            Value = SystemInfoFormatter.FormatDriver(SystemInfo.graphicsDeviceVersion);
        }
    }
}
