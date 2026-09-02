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
    public class OverlayViewBasic : OverlayViewBase
    {
        [Header("Prefabs")]
        [SerializeField] private StatViewBasic _statPrefab;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void CreateStatViews(IEnumerable<IStatProvider> statProviders)
        {
            foreach (var statProvider in statProviders)
            {
                var statView = Instantiate(_statPrefab, _container);
                ConfigureStatView(statView, statProvider);
                _statViews.Add(statProvider.Id, statView);
            }
        }

        public override void RefreshStats(IEnumerable<IStatProvider> statProviders)
        {
            foreach (var statProvider in statProviders)
            {
                if (_statViews.TryGetValue(statProvider.Id, out var statViewBasic))
                {
                    statViewBasic.SetValue($"{statProvider.FormattedValue} {statProvider.Unit}");
                    SetStatColoursBasedOnConditions(statViewBasic, statProvider);
                }
            }
        }  
    }
}
