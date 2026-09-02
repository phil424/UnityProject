/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using UnityEngine;

namespace JGT_Tools.PerfSight.HardwareInfo
{
    public class ApiInfo : IHardwareInfoProvider
    {
        public string Id => "api";
        public string Label => "API";
        public string Value { get; private set; }

        public void GetHardwareInfo()
        {
            Value = SystemInfo.graphicsDeviceType.ToString();
        }
    }
}
