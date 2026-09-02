/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

namespace JGT_Tools.PerfSight.Helpers
{
    public static class SystemInfoFormatter
    {
        public static string FormatCpu(string cpu)
        {
            cpu = cpu.Replace("AMD Ryzen", "R");
            cpu = cpu.Replace("Intel(R) Core(TM)", "i");
            cpu = cpu.Replace("with Radeon Graphics", "");
            cpu = cpu.Replace("(R)", "");
            cpu = cpu.Replace("(TM)", "");
            cpu = cpu.Replace("Processor", "");
            cpu = cpu.Replace("4-Core", "");
            cpu = cpu.Replace("6-Core", "");
            cpu = cpu.Replace("8-Core", "");
            cpu = cpu.Replace("10-Core", "");
            cpu = cpu.Replace("12-Core", "");

            return cpu.Trim();
        }

        public static string FormatGpu(string gpu)
        {
            gpu = gpu.Replace("NVIDIA GeForce", "");
            gpu = gpu.Replace("AMD Radeon", "");

            return gpu.Trim();
        }

        public static string FormatRam(long ramMb)
        {
            float ramGb = ramMb / 1024f;

            return $"{ramGb:F1} GB";
        }

        public static string FormatOs(string os)
        {
            if (os.Contains("Windows 11"))
                return "Win 11 64bit";

            if (os.Contains("Windows 10"))
                return "Win 10 64bit";

            return os;
        }

        public static string FormatDriver(string driver)
        {
            driver = driver.Replace("Direct3D", "");
            driver = driver.Replace("OpenGL", "");

            return driver.Trim();
        }
    }
}
