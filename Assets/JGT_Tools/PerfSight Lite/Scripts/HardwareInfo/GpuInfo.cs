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
    public class GpuInfo : IHardwareInfoProvider
    {
        public string Id => "gpu";
        public string Label => "GPU";
        public string Value { get; private set; }

        public void GetHardwareInfo()
        {
            Value = SystemInfoFormatter.FormatGpu(SystemInfo.graphicsDeviceName);
        }
    }
}
