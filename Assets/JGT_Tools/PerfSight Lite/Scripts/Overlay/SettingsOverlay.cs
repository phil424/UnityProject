/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JGT_Tools.PerfSight.Overlay
{
    public class SettingsOverlay : MonoBehaviour
    {
        [SerializeField] private PerfSightController _controller;
        [SerializeField] private TMP_Dropdown _overlayType;
        [SerializeField] private Toggle _overlayState;
        [SerializeField] private TMP_Dropdown _overlayPosition;
        [SerializeField] private Slider _overlayOpacity;
        [SerializeField] private Slider _overlayScale;

        private bool _editingAdvanced;

        private void Start()
        {
            if (_controller == null)
            {
                Debug.LogWarning($"{nameof(SettingsOverlay)}: PerfSightController reference is not assigned. Disabling.");
                enabled = false;
                return;
            }

            SetupOverlayTypeDropdown();

            if (_overlayType != null)
                _overlayType.onValueChanged.AddListener(OnOverlayTypeChanged);

            if (_overlayState != null)
                _overlayState.onValueChanged.AddListener(OnOverlayStateCanged);

            if (_overlayPosition != null)
                _overlayPosition.onValueChanged.AddListener(OnPositionChanged);

            if (_overlayOpacity != null)
                _overlayOpacity.onValueChanged.AddListener(OnOpacityChanged);

            if (_overlayScale != null)
                _overlayScale.onValueChanged.AddListener(OnScaleChanged);

            // default to editing Basic overlay initially (you can change to Advanced if desired)
            if (_overlayType != null)
            {
                _overlayType.value = 0;
                OnOverlayTypeChanged(_overlayType.value);
            }
            else
            {
                // If overlayType is not assigned still initialize UI from controller (default to Basic)
                _editingAdvanced = false;
                SetUiFromController();
            }
        }

        private void OnDestroy()
        {
            if (_overlayType != null) _overlayType.onValueChanged.RemoveListener(OnOverlayTypeChanged);
            if (_overlayState != null) _overlayState.onValueChanged.RemoveListener(OnOverlayStateCanged);
            if (_overlayPosition != null) _overlayPosition.onValueChanged.RemoveListener(OnPositionChanged);
            if (_overlayOpacity != null) _overlayOpacity.onValueChanged.RemoveListener(OnOpacityChanged);
            if (_overlayScale != null) _overlayScale.onValueChanged.RemoveListener(OnScaleChanged);
        }

        private void SetupOverlayTypeDropdown()
        {
            if (_overlayType == null) return;
            _overlayType.ClearOptions();
            _overlayType.AddOptions(new List<string> { "Basic", "Advanced" });
            _overlayType.RefreshShownValue();
        }

        private void PopulatePositionOptions()
        {
            if (_overlayPosition == null) return;

            _overlayPosition.ClearOptions();
            string[] names = _editingAdvanced
                ? Enum.GetNames(typeof(OverlayAdvancedAnchor))
                : Enum.GetNames(typeof(OverlayBasicAnchor));

            _overlayPosition.AddOptions(names.ToList());
            _overlayPosition.RefreshShownValue();
        }

        private void SetUiFromController()
        {
            if (_editingAdvanced)
            {
                PopulatePositionOptions();
                if (_overlayPosition != null) _overlayPosition.SetValueWithoutNotify((int)_controller.AdvancedAnchor);
                if (_overlayOpacity != null) _overlayOpacity.SetValueWithoutNotify(_controller.OverlayAdvancedOpacity);
                if (_overlayScale != null) _overlayScale.SetValueWithoutNotify(_controller.OverlayAdvancedScale);
                if (_overlayState != null) _overlayState.SetIsOnWithoutNotify(_controller.ShowAdvancedOverlay);
                if (_overlayPosition != null) _overlayPosition.RefreshShownValue();
            }
            else
            {
                PopulatePositionOptions();
                if (_overlayPosition != null) _overlayPosition.SetValueWithoutNotify((int)_controller.BasicAnchor);
                if (_overlayOpacity != null) _overlayOpacity.SetValueWithoutNotify(_controller.OverlayBasicOpacity);
                if (_overlayScale != null) _overlayScale.SetValueWithoutNotify(_controller.OverlayBasicScale);
                if (_overlayState != null) _overlayState.SetIsOnWithoutNotify(_controller.ShowBasicOverlay);
                if (_overlayPosition != null) _overlayPosition.RefreshShownValue();
            }
        }

        // UI callbacks -------------------------------------------------

        private void OnOverlayTypeChanged(int index)
        {
            _editingAdvanced = index == 1;
            SetUiFromController();
        }

        private void OnOverlayStateCanged(bool isOn)
        {
            if (_controller == null) return;

            if (_editingAdvanced)
            {
                _controller.ShowAdvancedOverlay = isOn;
            }
            else
            {
                _controller.ShowBasicOverlay = isOn;
            }
        }

        private void OnPositionChanged(int index)
        {
            if (_editingAdvanced)
            {
                _controller.SetAdvancedAnchor((OverlayAdvancedAnchor)index);
            }
            else
            {
                _controller.SetBasicAnchor((OverlayBasicAnchor)index);
            }
        }

        private void OnOpacityChanged(float value)
        {
            if (_editingAdvanced)
                _controller.SetAdvancedOpacity(value);
            else
                _controller.SetBasicOpacity(value);
        }

        private void OnScaleChanged(float value)
        {
            if (_editingAdvanced)
                _controller.SetAdvancedScale(value);
            else
                _controller.SetBasicScale(value);
        }
    }
}