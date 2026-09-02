/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using JGT_Tools.PerfSight.HardwareInfo;
using JGT_Tools.PerfSight.Helpers;
using JGT_Tools.PerfSight.Stats;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace JGT_Tools.PerfSight.Overlay
{
    public class PerfSightController : MonoBehaviour
    {
        #region Global Settings
        [Range(0.1f, 1f)]
        public float UpdateInterval = 0.25f;
#if ENABLE_INPUT_SYSTEM
        // New Input System keys.
        public Key OverlaySettingsKeybind = Key.F1;
#else
        // Legacy (KeyCode) toggle keys - kept for backwards compatibility.
        public KeyCode OverlaySettingsKeybindLegacy = KeyCode.F1;
#endif
        #endregion

        #region Overlay Settings
        public bool ShowOverlaySettings = true;
        [SerializeField] private SettingsOverlay _overlaySettings;
        #endregion

        #region Basic Overlay Settings
        public bool ShowBasicOverlay = true;
        [SerializeField] private OverlayBasicAnchor _basicAnchor = OverlayBasicAnchor.TopMiddle;
        [Range(0.25f, 1f)] public float OverlayBasicOpacity = 1f;
        [Range(0.5f, 1f)] public float OverlayBasicScale = 1f;

        [Space]
        [SerializeField] private OverlayViewBasic _overlayBasicView;
        [SerializeField] private RectTransform _overlayBasicRect;
        #endregion

        #region Advanced Overlay Settings
        public bool ShowAdvancedOverlay = true;
        [SerializeField] private OverlayAdvancedAnchor _advancedAnchor = OverlayAdvancedAnchor.Center;
        [Range(0.25f, 1f)] public float OverlayAdvancedOpacity = 1f;
        [Range(0.5f, 1f)] public float OverlayAdvancedScale = 1f;

        [Space]
        [SerializeField] private OverlayViewAdvanced _overlayAdvancedView;
        [SerializeField] private RectTransform _overlayAdvancedRect;
        #endregion

        #region Stat Condition Settings
        [SerializeField] private float _optimalFrameRate = 60;
        [SerializeField] private float _warningFrameRate = 30;

        [SerializeField] private float _optimalFrameTime = 16.66f; // in ms
        [SerializeField] private float _warningFrameTime = 33.33f; // in ms

        [SerializeField] private int _optimalMemoryUsage = 500; // in MB
        [SerializeField] private int _warningMemoryUsage = 1000; // in MB
#if UNITY_EDITOR
        [SerializeField] private int _optimalGcAlloc = 0; // in Bytes
        [SerializeField] private int _warningGcAlloc = 1024; // in Bytes
        [Tooltip("Due to Editor overhead, this value should be used as a buffer and will not be used in Unity Builds.")]
        [SerializeField] private int _editorGcAllocOverhead = 10000; // Bytes
#else
        private int _optimalGcAlloc = 0; // Bytes
        private int _warningGcAlloc = 1024; // Bytes
#endif

        [SerializeField] private int _optimalDrawcalls = 100;
        [SerializeField] private int _warningDrawcalls = 300;

        [SerializeField] private int _optimalTriangles = 500000;
        [SerializeField] private int _warningTriangles = 1000000;

        [SerializeField] private int _optimalVertices = 250000;
        [SerializeField] private int _warningVertices = 500000;
        #endregion

        private OverlayAdvancedAnchor _currentAdvancedAnchor;
        private OverlayBasicAnchor _currentBasicAnchor;

        private List<IStatProvider> _statCollection = new List<IStatProvider>();
        private List<IHardwareInfoProvider> _hardwareInfoCollection = new List<IHardwareInfoProvider>();

        private Vector2 _offset = new Vector2(10f, 10f);
        private float _timer;

        private void Awake()
        {
            InitialisePerformanceStats();
            InitialiseHardwareInfo();
        }

        private void Start()
        {
            if (!ValidateReferences())
                return;

            _currentAdvancedAnchor = _advancedAnchor;
            _currentBasicAnchor = _basicAnchor;

            ApplyBasicAnor();
            ApplyAdvancedAnchor();

            _overlayAdvancedView.SetOpacity(OverlayAdvancedOpacity);
            _overlayBasicView.SetOpacity(OverlayBasicOpacity);
            SetOverlayScale(_overlayAdvancedRect, OverlayAdvancedScale);
            SetOverlayScale(_overlayBasicRect, OverlayBasicScale);

            _overlayBasicView.CreateStatViews(_statCollection);
            _overlayAdvancedView.CreateStatViews(_statCollection);
            _overlayAdvancedView.CreateHardwareInfoViews(_hardwareInfoCollection);

            SetHardwareInfoValues();
            _overlayAdvancedView.RefreshHardwareInfo(_hardwareInfoCollection);
        }

        private void Update()
        {
            UpdateVisibility();
            UpdateAnchors();
            UpdateOpacity();
            UpdateScale();
        }

        private void LateUpdate()
        {
            UpdateStatValues();
        }

        private void InitialiseHardwareInfo()
        {
            _hardwareInfoCollection.AddRange(new List<IHardwareInfoProvider>
            {
                new ResolutionInfo(),
                new ApiInfo(),
                new GpuInfo(),
                new CpuInfo(),
                new RamInfo(),
                new OSInfo(),
                new DriverInfo()
            });
        }

        private void InitialisePerformanceStats()
        {
            _statCollection.AddRange(new List<IStatProvider>
            {
                new FpsStat(_optimalFrameRate, _warningFrameRate),
                new FrameTimeStat(_optimalFrameTime, _warningFrameTime),
                new MemoryStat(_optimalMemoryUsage, _warningMemoryUsage),
#if UNITY_EDITOR
                new GcStat(_optimalGcAlloc + _editorGcAllocOverhead,
                    _warningGcAlloc + _editorGcAllocOverhead),
#else
                new GcStat(_optimalGcAlloc, _warningGcAlloc),
#endif
                new DrawcallStat(_optimalDrawcalls, _warningDrawcalls),
                new TrianglesStat(_optimalTriangles, _warningTriangles),
                new VerticesStat(_optimalVertices, _warningVertices)
            });
        }

        private void SetHardwareInfoValues()
        {
            foreach (var hardwareInfo in _hardwareInfoCollection)
                hardwareInfo.GetHardwareInfo();
        }

        private void SetStatValues()
        {
            foreach (var stat in _statCollection)
                stat.CalculateStat();
        }

        private void UpdateStatValues()
        {
            _timer += Time.deltaTime;
            if (_timer >= UpdateInterval)
            {
                SetStatValues();
                _overlayAdvancedView.RefreshStats(_statCollection);
                _overlayBasicView.RefreshStats(_statCollection);
                _timer = 0f;
            }
        }

        private void UpdateVisibility()
        {
#if ENABLE_INPUT_SYSTEM
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (OverlaySettingsKeybind != Key.None)
                {
                    var overlaySettingControl = keyboard[OverlaySettingsKeybind];
                    if (overlaySettingControl != null && overlaySettingControl.wasPressedThisFrame)
                        ShowOverlaySettings = !ShowOverlaySettings;
                }
            }
#else
            // Legacy Input fallback
            if (OverlaySettingsKeybindLegacy != KeyCode.None && Input.GetKeyDown(OverlaySettingsKeybindLegacy))
            {
                ShowOverlaySettings = !ShowOverlaySettings;
            }
#endif

            if (ShowOverlaySettings != _overlaySettings.gameObject.activeSelf)
            {
                _overlaySettings.gameObject.SetActive(ShowOverlaySettings);
            }

            if (_overlayAdvancedView != null && ShowAdvancedOverlay != _overlayAdvancedView.gameObject.activeSelf)
            {
                _overlayAdvancedView.gameObject.SetActive(ShowAdvancedOverlay);
            }

            if (_overlayBasicView != null && ShowBasicOverlay != _overlayBasicView.gameObject.activeSelf)
            {
                _overlayBasicView.gameObject.SetActive(ShowBasicOverlay);
            }
        }

        private void ApplyAdvancedAnchor()
        {
            switch (_advancedAnchor)
            {
                case OverlayAdvancedAnchor.TopLeft:
                    RectAnchor.SetAnchor(_overlayAdvancedRect, Vector2.up);
                    RectAnchor.SetPivot(_overlayAdvancedRect, new Vector2(0f, 1f));
                    _overlayAdvancedRect.anchoredPosition = new Vector2(_offset.x, -_offset.y);
                    break;

                case OverlayAdvancedAnchor.TopRight:
                    RectAnchor.SetAnchor(_overlayAdvancedRect, Vector2.one);
                    RectAnchor.SetPivot(_overlayAdvancedRect, Vector2.one);
                    _overlayAdvancedRect.anchoredPosition = new Vector2(-_offset.x, -_offset.y);
                    break;

                case OverlayAdvancedAnchor.Center:
                    RectAnchor.SetAnchor(_overlayAdvancedRect, new Vector2(0.5f, 0.5f));
                    RectAnchor.SetPivot(_overlayAdvancedRect, new Vector2(0.5f, 0.5f));
                    _overlayAdvancedRect.anchoredPosition = Vector2.zero;
                    break;

                case OverlayAdvancedAnchor.BottomLeft:
                    RectAnchor.SetAnchor(_overlayAdvancedRect, Vector2.zero);
                    RectAnchor.SetPivot(_overlayAdvancedRect, Vector2.zero);
                    _overlayAdvancedRect.anchoredPosition = _offset;
                    break;
            }
        }

        private void ApplyBasicAnor()
        {
            switch (_basicAnchor)
            {
                case OverlayBasicAnchor.TopLeft:
                    RectAnchor.SetAnchor(_overlayBasicRect, Vector2.up);
                    RectAnchor.SetPivot(_overlayBasicRect, new Vector2(0f, 1f));
                    _overlayBasicRect.anchoredPosition = new Vector2(_offset.x, -_offset.y);
                    break;

                case OverlayBasicAnchor.TopRight:
                    RectAnchor.SetAnchor(_overlayBasicRect, Vector2.one);
                    RectAnchor.SetPivot(_overlayBasicRect, Vector2.one);
                    _overlayBasicRect.anchoredPosition = new Vector2(-_offset.x, -_offset.y);
                    break;

                case OverlayBasicAnchor.TopMiddle:
                    RectAnchor.SetAnchor(_overlayBasicRect, new Vector2(0.5f, 1f));
                    RectAnchor.SetPivot(_overlayBasicRect, new Vector2(0.5f, 1f));
                    _overlayBasicRect.anchoredPosition = new Vector2(0f, -_offset.y);
                    break;

                case OverlayBasicAnchor.BottomLeft:
                    RectAnchor.SetAnchor(_overlayBasicRect, Vector2.zero);
                    RectAnchor.SetPivot(_overlayBasicRect, Vector2.zero);
                    _overlayBasicRect.anchoredPosition = _offset;
                    break;

                case OverlayBasicAnchor.BottomMiddle:
                    RectAnchor.SetAnchor(_overlayBasicRect, new Vector2(0.5f, 0f));
                    RectAnchor.SetPivot(_overlayBasicRect, new Vector2(0.5f, 0f));
                    _overlayBasicRect.anchoredPosition = new Vector2(0f, _offset.y);
                    break;
            }
        }

        private void UpdateAnchors()
        {
            if (_advancedAnchor != _currentAdvancedAnchor || _basicAnchor != _currentBasicAnchor)
            {
                _currentAdvancedAnchor = _advancedAnchor;
                ApplyAdvancedAnchor();

                _currentBasicAnchor = _basicAnchor;
                ApplyBasicAnor();
            }
        }

        private void UpdateOpacity()
        {
            if (!Mathf.Approximately(OverlayAdvancedOpacity, _overlayAdvancedView.GetOpacity()))
            {
                _overlayAdvancedView.SetOpacity(OverlayAdvancedOpacity);
            }

            if (!Mathf.Approximately(OverlayBasicOpacity, _overlayBasicView.GetOpacity()))
            {
                _overlayBasicView.SetOpacity(OverlayBasicOpacity);
            }
        }

        private void SetOverlayScale(RectTransform overlayRect, float scale)
        {
            overlayRect.localScale = new Vector3(scale, scale, 1f);
        }

        private void UpdateScale()
        {
            if (!Mathf.Approximately(OverlayAdvancedScale, _overlayAdvancedRect.localScale.x) ||
                !Mathf.Approximately(OverlayAdvancedScale, _overlayAdvancedRect.localScale.y))
            {
                SetOverlayScale(_overlayAdvancedRect, OverlayAdvancedScale);
            }

            if (!Mathf.Approximately(OverlayBasicScale, _overlayBasicRect.localScale.x) ||
                !Mathf.Approximately(OverlayBasicScale, _overlayBasicRect.localScale.y))
            {
                SetOverlayScale(_overlayBasicRect, OverlayBasicScale);
            }
        }

        public OverlayAdvancedAnchor AdvancedAnchor => _advancedAnchor;
        public OverlayBasicAnchor BasicAnchor => _basicAnchor;

        public void SetAdvancedAnchor(OverlayAdvancedAnchor anchor)
        {
            _advancedAnchor = anchor;
            _currentAdvancedAnchor = anchor;
            ApplyAdvancedAnchor();
        }

        public void SetBasicAnchor(OverlayBasicAnchor anchor)
        {
            _basicAnchor = anchor;
            _currentBasicAnchor = anchor;
            ApplyBasicAnor();
        }

        public void SetAdvancedOpacity(float value)
        {
            OverlayAdvancedOpacity = Mathf.Clamp(value, 0.25f, 1f);
            _overlayAdvancedView?.SetOpacity(OverlayAdvancedOpacity);
        }

        public void SetBasicOpacity(float value)
        {
            OverlayBasicOpacity = Mathf.Clamp(value, 0.25f, 1f);
            _overlayBasicView?.SetOpacity(OverlayBasicOpacity);
        }
        
        public void SetAdvancedScale(float value)
        {
            OverlayAdvancedScale = Mathf.Clamp(value, 0.5f, 1f);
            SetOverlayScale(_overlayAdvancedRect, OverlayAdvancedScale);
        }

        public void SetBasicScale(float value)
        {
            OverlayBasicScale = Mathf.Clamp(value, 0.5f, 1f);
            SetOverlayScale(_overlayBasicRect, OverlayBasicScale);
        }

        private bool ValidateReferences()
        {
            if (_overlayAdvancedView == null)
            {
                Debug.LogWarning($"{nameof(PerfSightController)}: _overlayView is not assigned. Disabling component.");
                enabled = false;
                return false;
            }

            if (_overlayAdvancedRect == null)
            {
                Debug.LogWarning($"{nameof(PerfSightController)}: _overlayRect is not assigned. Disabling component.");
                enabled = false;
                return false;
            }

            return true;
        }
    }

    public enum OverlayAdvancedAnchor
    {
        TopLeft,
        TopRight,
        Center,
        BottomLeft
    }

    public enum OverlayBasicAnchor
    {
        TopLeft,
        TopMiddle,
        TopRight,
        BottomLeft,
        BottomMiddle
    }
}