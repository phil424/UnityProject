/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using JGT_Tools.PerfSight.HardwareInfo;
using JGT_Tools.PerfSight.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace JGT_Tools.PerfSight.Overlay
{
    public class OverlayViewAdvanced : OverlayViewBase
    {
        [Header("Prefabs")]
        [SerializeField] private StatViewAdvanced _statPrefab;
        [SerializeField] private HardwareInfoView _hardwareInfoPrefab;
        [SerializeField] private GameObject _dividerPrefab;
        [SerializeField] private GameObject _subHeaderPrefab;

        [Header("Icons")]
        [SerializeField] private List<StatIconMapping> _iconMappings;

        private readonly Dictionary<string, HardwareInfoView> _hardwareInfoViews = new();
        private readonly Dictionary<string, Sprite> _iconLookup = new();

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            foreach (var mapping in _iconMappings)
            {
                _iconLookup[mapping.Id] = mapping.Icon;
            }
        }

        public override void CreateStatViews(IEnumerable<IStatProvider> statProviders)
        {
            foreach (var statProvider in statProviders)
            {
                var statView = Instantiate(_statPrefab, _container);
                ConfigureStatView(statView, statProvider);                
                _statViews.Add(statProvider.Id, statView);
            }

            Instantiate(_dividerPrefab, _container);
        }

        protected override void ConfigureStatView(StatViewBase view, IStatProvider provider)
        {
            base.ConfigureStatView(view, provider);
            if (view is StatViewAdvanced statViewAdvanced)
            {
                statViewAdvanced.SetIcon(GetIcon(provider.Id));
            }
        }

        public override void RefreshStats(IEnumerable<IStatProvider> statProviders)
        {
            foreach (var statProvider in statProviders)
            {
                if (_statViews.TryGetValue(statProvider.Id, out var statViewAdvanced))
                {
                    statViewAdvanced.SetValue($"{statProvider.FormattedValue} {statProvider.Unit}");
                    SetStatColoursBasedOnConditions(statViewAdvanced, statProvider);
                }
            }
        }

        public void CreateHardwareInfoViews(IEnumerable<IHardwareInfoProvider> hardwareInfoProviders)
        {
            Instantiate(_subHeaderPrefab, _container);

            foreach (var hardwareInfoProvider in hardwareInfoProviders)
            {
                var hardwareInfoView = Instantiate(_hardwareInfoPrefab, _container);
                hardwareInfoView.SetIcon(GetIcon(hardwareInfoProvider.Id));
                hardwareInfoView.SetLabel(hardwareInfoProvider.Label);
                _hardwareInfoViews.Add(hardwareInfoProvider.Id, hardwareInfoView);
            }

            Instantiate(_dividerPrefab, _container);
        }

        public void RefreshHardwareInfo(IEnumerable<IHardwareInfoProvider> hardwareInfoProviders)
        {
            foreach (var hardwareInfoProvider in hardwareInfoProviders)
            {
                if (_hardwareInfoViews.TryGetValue(hardwareInfoProvider.Id, out var hardwareInfoView))
                {
                    hardwareInfoView.SetValue(hardwareInfoProvider.Value);
                }
            }
        }

        private Sprite GetIcon(string statId)
        {
            return _iconLookup.TryGetValue(statId, out var icon)
                ? icon
                : null;
        }
    }
}
