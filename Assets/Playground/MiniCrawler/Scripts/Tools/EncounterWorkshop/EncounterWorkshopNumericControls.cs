using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MiniCrawler.Tools
{
    public sealed class EncounterWorkshopNumericControls
    {
        private readonly Dictionary<string, string> buffers = new();

        public int DrawInt(
            string key,
            string label,
            int value,
            int minimum,
            int maximum,
            float labelWidth = 145f,
            float sliderWidth = 280f,
            float fieldWidth = 70f)
        {
            string controlName = $"WorkshopInt_{key}";

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelWidth));

            int sliderValue = Mathf.Clamp(
                Mathf.RoundToInt(
                    GUILayout.HorizontalSlider(
                        value,
                        minimum,
                        maximum,
                        GUILayout.Width(sliderWidth)
                    )
                ),
                minimum,
                maximum
            );

            bool sliderChanged = sliderValue != value;

            if (sliderChanged)
            {
                value = sliderValue;
                buffers[key] = value.ToString(CultureInfo.InvariantCulture);
            }

            bool wasFocused = GUI.GetNameOfFocusedControl() == controlName;

            if (!wasFocused && !sliderChanged)
                buffers[key] = value.ToString(CultureInfo.InvariantCulture);

            GUI.SetNextControlName(controlName);

            string text = GUILayout.TextField(
                GetBuffer(key, value.ToString(CultureInfo.InvariantCulture)),
                GUILayout.Width(fieldWidth)
            );

            buffers[key] = text;

            if (int.TryParse(
                    text,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int parsed))
            {
                value = Mathf.Clamp(parsed, minimum, maximum);
            }

            bool isFocused = GUI.GetNameOfFocusedControl() == controlName;

            if (!isFocused)
                buffers[key] = value.ToString(CultureInfo.InvariantCulture);

            GUILayout.EndHorizontal();

            return value;
        }

        public float DrawFloat(
            string key,
            string label,
            float value,
            float minimum,
            float maximum,
            float sliderStep,
            int decimals = 2,
            float labelWidth = 145f,
            float sliderWidth = 280f,
            float fieldWidth = 70f)
        {
            string controlName = $"WorkshopFloat_{key}";
            string format = $"F{Mathf.Clamp(decimals, 0, 4)}";

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelWidth));

            float sliderValue = GUILayout.HorizontalSlider(
                value,
                minimum,
                maximum,
                GUILayout.Width(sliderWidth)
            );

            sliderValue = Snap(sliderValue, sliderStep);
            sliderValue = Mathf.Clamp(sliderValue, minimum, maximum);

            bool sliderChanged = !Mathf.Approximately(sliderValue, value);

            if (sliderChanged)
            {
                value = sliderValue;
                buffers[key] = value.ToString(format, CultureInfo.InvariantCulture);
            }

            bool wasFocused = GUI.GetNameOfFocusedControl() == controlName;

            if (!wasFocused && !sliderChanged)
                buffers[key] = value.ToString(format, CultureInfo.InvariantCulture);

            GUI.SetNextControlName(controlName);

            string text = GUILayout.TextField(
                GetBuffer(key, value.ToString(format, CultureInfo.InvariantCulture)),
                GUILayout.Width(fieldWidth)
            );

            buffers[key] = text;

            if (float.TryParse(
                    text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float parsed))
            {
                // Direct entry is deliberately NOT snapped.
                // The slider snaps; the numeric field permits exact values.
                value = Mathf.Clamp(parsed, minimum, maximum);
            }

            bool isFocused = GUI.GetNameOfFocusedControl() == controlName;

            if (!isFocused)
                buffers[key] = value.ToString(format, CultureInfo.InvariantCulture);

            GUILayout.EndHorizontal();

            return value;
        }

        public float DrawFloatField(
            string key,
            string label,
            float value,
            float minimum,
            float maximum,
            int decimals = 2,
            float labelWidth = 145f,
            float fieldWidth = 90f)
        {
            string controlName = $"WorkshopFloatField_{key}";
            string format = $"F{Mathf.Clamp(decimals, 0, 4)}";

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelWidth));

            bool wasFocused = GUI.GetNameOfFocusedControl() == controlName;

            if (!wasFocused)
                buffers[key] = value.ToString(format, CultureInfo.InvariantCulture);

            GUI.SetNextControlName(controlName);

            string text = GUILayout.TextField(
                GetBuffer(key, value.ToString(format, CultureInfo.InvariantCulture)),
                GUILayout.Width(fieldWidth)
            );

            buffers[key] = text;

            if (float.TryParse(
                    text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out float parsed))
            {
                value = Mathf.Clamp(parsed, minimum, maximum);
            }

            bool isFocused = GUI.GetNameOfFocusedControl() == controlName;

            if (!isFocused)
                buffers[key] = value.ToString(format, CultureInfo.InvariantCulture);

            GUILayout.EndHorizontal();

            return value;
        }

        private string GetBuffer(string key, string fallback)
        {
            if (!buffers.TryGetValue(key, out string value))
            {
                value = fallback;
                buffers[key] = value;
            }

            return value;
        }

        private static float Snap(float value, float step)
        {
            if (step <= 0f)
                return value;

            return Mathf.Round(value / step) * step;
        }
    }
}