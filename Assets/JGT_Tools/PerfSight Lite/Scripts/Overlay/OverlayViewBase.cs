/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using JGT_Tools.PerfSight.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace JGT_Tools.PerfSight.Overlay
{
    public abstract class OverlayViewBase : MonoBehaviour
    {
        [Header("Transform Container")]
        [SerializeField] protected Transform _container;

        protected CanvasGroup _canvasGroup;
        protected readonly Dictionary<string, StatViewBase> _statViews = new();

        public abstract void CreateStatViews(IEnumerable<IStatProvider> statProviders);
        public abstract void RefreshStats(IEnumerable<IStatProvider> statProviders);
        protected virtual void ConfigureStatView(StatViewBase view, IStatProvider provider)
        {
            view.SetLabel(provider.Label);
            view.SetValueColour(provider.ValueColour);
        }

        protected void SetStatColoursBasedOnConditions(StatViewBase statView, IStatProvider statProvider)
        {
            const string green = "#39FF88";
            const string amber = "#FFD166";
            const string red = "#FF3939";

            var value = statProvider.Value;
            var optimal = statProvider.OptimalValue;
            var warning = statProvider.WarningValue;

            if (statProvider.HigherIsBetter)
            {
                if (value >= optimal)
                {
                    statView.SetValueColour(green);
                }
                else if (value >= warning)
                {
                    statView.SetValueColour(amber);
                }
                else
                {
                    statView.SetValueColour(red);
                }
            }
            else
            {
                if (value <= optimal)
                {
                    statView.SetValueColour(green);
                }
                else if (value <= warning)
                {
                    statView.SetValueColour(amber);
                }
                else
                {
                    statView.SetValueColour(red);
                }
            }
        }

        public void SetOpacity(float opacity)
        {
            var canvasOpacity = Mathf.Clamp01(opacity);
            _canvasGroup.alpha = canvasOpacity;
        }

        public float GetOpacity()
        {
            return _canvasGroup.alpha;
        }
    }
}
