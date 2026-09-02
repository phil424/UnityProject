/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JGT_Tools.PerfSight.HardwareInfo
{
    public class HardwareInfoView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _value;

        public void SetIcon(Sprite icon)
        {
            _icon.sprite = icon;
        }

        public void SetLabel(string label)
        {
            _label.SetText(label);
        }

        public void SetValue(string value)
        {
            _value.SetText(value);
        }
    }
}
