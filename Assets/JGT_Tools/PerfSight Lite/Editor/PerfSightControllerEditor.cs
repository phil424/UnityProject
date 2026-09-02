/*
 * PerfSight - Performance Profiling Toolkit
 * Copyright (c) 2026 JGT Tools
 *
 * This software is provided under the Unity Asset Store EULA.
 * Unauthorized redistribution or resale is prohibited.
 */

using UnityEditor;
using UnityEngine;
using JGT_Tools.PerfSight.Overlay;

namespace JGT_Tools.PerfSight.EditorExtensions
{
    [CustomEditor(typeof(PerfSightController))]
    public class PerfSightControllerEditor : Editor
    {
        private bool _showGlobalSettings = false;
        private bool _showOverlaySettings = true;
        private bool _showBasicOverlay = true;
        private bool _showAdvancedOverlay = true;
        
        private bool _showFPSSettings = false;
        private bool _showMemorySettings = false;
        private bool _showGCAllocSettings = false;
        private bool _showDrawCallSettings = false;
        private bool _showTriangleSettings = false;
        private bool _showVerticesSettings = false;

        private Texture2D _banner;

        private static readonly Color AccentColor =
            new Color(0.15f, 0.65f, 1f);

        private void OnEnable()
        {
            _banner = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/JGT_Tools/PerfSight Lite/Editor/Images/PerfSight-Lite_banner.png");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawBanner();

            GUILayout.Space(5);

            DrawStatusPanel();

            GUILayout.Space(8);

            DrawGlobalSettings();

            GUILayout.Space(5);

            DrawOverlaySettings();

            GUILayout.Space(5);

            DrawBasicOverlaySettings();

            GUILayout.Space(5);

            DrawAdvancedOverlaySettings();

            GUILayout.Space(5);

            DrawStatConditions();

            serializedObject.ApplyModifiedProperties();
        }

        #region Sections

        private void DrawGlobalSettings()
        {
            DrawSectionHeader("GLOBAL SETTINGS");

            _showGlobalSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showGlobalSettings,
                    "Settings");

            if (_showGlobalSettings)
            {
                GUILayout.BeginVertical("box");

                DrawProperty("UpdateInterval");

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawOverlaySettings()
        {
            DrawSectionHeader("OVERLAY SETTINGS");

            _showOverlaySettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showOverlaySettings,
                    "Settings");

            if (_showOverlaySettings)
            {
                GUILayout.BeginVertical("box");

                DrawProperty("ShowOverlaySettings");

                EditorGUILayout.LabelField("New Unity Input System Hotkeys", EditorStyles.boldLabel);
                DrawProperty("OverlaySettingsKeybind");
                
                GUILayout.Space(5);

                EditorGUILayout.LabelField("Legacy Unity Input System Hotkeys", EditorStyles.boldLabel);
                DrawProperty("OverlaySettingsKeybindLegacy");

                GUILayout.Space(5);

                EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
                DrawProperty("_overlaySettings");

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawBasicOverlaySettings()
        {
            DrawSectionHeader("BASIC OVERLAY");

            _showBasicOverlay =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showBasicOverlay,
                    "Settings");

            if (_showBasicOverlay)
            {
                GUILayout.BeginVertical("box");

                DrawProperty("ShowBasicOverlay");
                DrawProperty("_basicAnchor");
                DrawProperty("OverlayBasicOpacity");
                DrawProperty("OverlayBasicScale");

                GUILayout.Space(5);

                EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
                DrawProperty("_overlayBasicView");
                DrawProperty("_overlayBasicRect");

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawAdvancedOverlaySettings()
        {
            DrawSectionHeader("ADVANCED OVERLAY");

            _showAdvancedOverlay =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showAdvancedOverlay,
                    "Settings");

            if (_showAdvancedOverlay)
            {
                GUILayout.BeginVertical("box");

                DrawProperty("ShowAdvancedOverlay");
                DrawProperty("_advancedAnchor");
                DrawProperty("OverlayAdvancedOpacity");
                DrawProperty("OverlayAdvancedScale");

                GUILayout.Space(5);

                EditorGUILayout.LabelField("Components", EditorStyles.boldLabel);
                DrawProperty("_overlayAdvancedView");
                DrawProperty("_overlayAdvancedRect");

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawStatConditions()
        {
            DrawSectionHeader("STAT CONDITIONS");

            #region FPS
            _showFPSSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showFPSSettings,
                    "Fps/FrameTime");

            if (_showFPSSettings)
            {
                DrawMetricThreshold(
                    "FPS",
                    "_optimalFrameRate",
                    "_warningFrameRate",
                    "fps");

                GUILayout.Space(5);

                DrawMetricThreshold(
                    "Frame Time",
                    "_optimalFrameTime",
                    "_warningFrameTime",
                    "ms");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region Memory
            _showMemorySettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showMemorySettings,
                    "Memory");


            if (_showMemorySettings)
            {
                DrawMetricThreshold(
                    "Memory Usage",
                    "_optimalMemoryUsage",
                    "_warningMemoryUsage",
                    "MB");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region GC Alloc
            _showGCAllocSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showGCAllocSettings,
                    "Gc Alloc");


            if (_showGCAllocSettings)
            {
                DrawMetricThreshold(
                    "GC Alloc",
                    "_optimalGcAlloc",
                    "_warningGcAlloc",
                    "B");

                DrawProperty("_editorGcAllocOverhead");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region Draw Calls
            _showDrawCallSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showDrawCallSettings,
                    "Drawcalls");


            if (_showDrawCallSettings)
            {
                DrawMetricThreshold(
                    "Draw Calls",
                    "_optimalDrawcalls",
                    "_warningDrawcalls");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region Triangles
            _showTriangleSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showTriangleSettings,
                    "Triangles");


            if (_showTriangleSettings)
            {
                DrawMetricThreshold(
                    "Triangles",
                    "_optimalTriangles",
                    "_warningTriangles");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion

            #region Vertices
            _showVerticesSettings =
                EditorGUILayout.BeginFoldoutHeaderGroup(
                    _showVerticesSettings,
                    "Vertices");


            if (_showVerticesSettings)
            {
                DrawMetricThreshold(
                    "Vertices",
                    "_optimalVertices",
                    "_warningVertices");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            #endregion
        }

        #endregion

        #region Status Panel

        private void DrawStatusPanel()
        {
            PerfSightController controller =
                (PerfSightController)target;

            GUILayout.BeginVertical("box");

            GUILayout.Label(
                "STATUS",
                EditorStyles.boldLabel);

            DrawStatus(
                "Basic Overlay Enabled",
                controller.ShowBasicOverlay);

            DrawStatus(
                "Advanced Overlay Enabled",
                controller.ShowAdvancedOverlay);

            DrawStatus(
                "Overlay Settings Enabled",
                controller.ShowOverlaySettings);

            GUILayout.EndVertical();
        }

        private void DrawStatus(string label, bool enabled)
        {
            GUIStyle style =
                new GUIStyle(EditorStyles.label);

            style.normal.textColor =
                enabled ? Color.green : Color.red;

            GUILayout.Label(
                $"{(enabled ? "✓" : "✗")} {label}",
                style);
        }

        #endregion

        #region UI Helpers

        private void DrawBanner()
        {
            if (_banner == null)
                return;

            Rect rect = GUILayoutUtility.GetRect(
                EditorGUIUtility.currentViewWidth,
                72,
                GUILayout.ExpandWidth(true));

            GUI.DrawTexture(
                rect,
                _banner,
                ScaleMode.ScaleToFit);
        }

        private void DrawSectionHeader(string title)
        {
            GUILayout.Space(3);

            Rect rect = EditorGUILayout.GetControlRect(
                false,
                2);

            EditorGUI.DrawRect(
                rect,
                AccentColor);

            GUILayout.Space(2);

            GUIStyle style =
                new GUIStyle(EditorStyles.boldLabel);

            style.fontSize = 11;

            GUILayout.Label(
                title,
                style);

            GUILayout.Space(2);
        }

        private void DrawProperty(string propertyName)
        {
            SerializedProperty property =
                serializedObject.FindProperty(
                    propertyName);

            if (property != null)
            {
                EditorGUILayout.PropertyField(
                    property,
                    true);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"Property '{propertyName}' not found.",
                    MessageType.Warning);
            }
        }

        private void DrawMetricThreshold(string title, string optimalProperty,
            string warningProperty, string unit = "")
        {
            GUILayout.BeginVertical("box");

            GUILayout.Label(
                title,
                EditorStyles.boldLabel);

            SerializedProperty optimal =
                serializedObject.FindProperty(optimalProperty);

            SerializedProperty warning =
                serializedObject.FindProperty(warningProperty);

            Color oldColor = GUI.color;

            GUI.color = Color.green;

            EditorGUILayout.PropertyField(
                optimal,
                new GUIContent("✓ Good"));

            GUI.color = new Color(1f, 0.8f, 0f);

            EditorGUILayout.PropertyField(
                warning,
                new GUIContent("⚠ Warning"));

            GUI.color = oldColor;

            GUILayout.EndVertical();
        }

        #endregion
    }
}