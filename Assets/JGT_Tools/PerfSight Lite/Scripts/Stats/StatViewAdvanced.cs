/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using UnityEngine;
using UnityEngine.UI;

namespace JGT_Tools.PerfSight.Stats
{
    public class StatViewAdvanced : StatViewBase
    {
        [SerializeField] private Image _icon;

        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }
    }
}
