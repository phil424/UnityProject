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
    public class OSInfo : IHardwareInfoProvider
    {
        public string Id => "os";
        public string Label => "OS";
        public string Value { get; private set; }

        public void GetHardwareInfo()
        {
            Value = SystemInfoFormatter.FormatOs(SystemInfo.operatingSystem);
        }
    }
}
