/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using TMPro;
using UnityEngine;

namespace JGT_Tools.PerfSight.Stats
{
    public class StatViewBase : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _value;

        public void SetLabel(string label)
        {
            _label.SetText(label);
        }

        public void SetValue(string valueWithSuffix)
        {
            _value.SetText(valueWithSuffix);
        }

        public void SetValueColour(string hexadecimal)
        {
            ColorUtility.TryParseHtmlString(hexadecimal, out Color _color);
            _value.color = _color;
        }
    }
}
