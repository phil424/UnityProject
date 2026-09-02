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
    public class ResolutionInfo : IHardwareInfoProvider
    {
        public string Id => "resolution";
        public string Label => "RES";
        public string Value { get; private set; }

        public void GetHardwareInfo()
        {
            Value = $"{Screen.currentResolution.width} x {Screen.currentResolution.height}";
        }
    }
}
