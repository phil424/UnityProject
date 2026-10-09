using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Progress;
using MiniCrawler.Tools.ContentAuthoring;
using UnityEditor;
using UnityEngine;

namespace MiniCrawler.Tools.MasteryAuthoring
{
    internal sealed class CharacterMasteryGraphEditor
    {
        public static readonly Vector2 NodeSize = new(190f, 88f);

        private const float CanvasPadding = 80f;
        private const float GridStepX = 230f;
        private const float GridStepY = 120f;

        private PartyMemberDefinition character;
        private CharacterMasteryNodeDefinition selectedNode;
        private CharacterMasteryNodeDefinition draggingNode;

        private Vector2 graphScrollPosition;
        private Vector2 dragOffset;
        private bool centerViewRequested = true;

        private CharacterMasteryDefinition Mastery => character != null ? character.MasteryDefinition : null;

        public CharacterMasteryNodeDefinition SelectedNode => selectedNode;

        public void SetCharacter(PartyMemberDefinition nextCharacter)
        {
            if (character == nextCharacter)
                return;

            character = nextCharacter;
            selectedNode = null;
            draggingNode = null;
            centerViewRequested = true;
        }

        public bool OwnsNode(CharacterMasteryNodeDefinition node)
        {
            return node != null && Mastery != null && Mastery.Contains(node);
        }

        public void SelectNode(CharacterMasteryNodeDefinition node)
        {
            if (node == null)
            {
                selectedNode = null;
                return;
            }

            if (!OwnsNode(node))
                return;

            selectedNode = node;
        }

        public void HandleProjectChange()
        {
            if (selectedNode != null && !OwnsNode(selectedNode))
                selectedNode = null;

            if (draggingNode != null && !OwnsNode(draggingNode))
                draggingNode = null;
        }

        public void Center()
        {
            centerViewRequested = true;
        }

        public void ShowCreateMenuAtOpenPosition()
        {
            if (Mastery == null)
                return;

            ShowCreateMenu(FindOpenPosition());
        }

        public bool Draw()
        {
            if (Mastery == null)
                return false;

            bool layoutChanged = false;

            Rect viewportRect = GUILayoutUtility.GetRect(320f, 10000f, 360f, 10000f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            Vector2 treeSize = Mastery.TreeSize;
            Rect contentRect = new(0f, 0f, treeSize.x + CanvasPadding * 2f, treeSize.y + CanvasPadding * 2f);

            if (centerViewRequested)
            {
                graphScrollPosition = new Vector2(Mathf.Max(0f, (contentRect.width - viewportRect.width) * 0.5f), Mathf.Max(0f, (contentRect.height - viewportRect.height) * 0.5f));

                centerViewRequested = false;
            }

            graphScrollPosition = GUI.BeginScrollView(viewportRect, graphScrollPosition, contentRect);

            Rect canvasRect = new(CanvasPadding, CanvasPadding, treeSize.x, treeSize.y);

            EditorGUI.DrawRect(canvasRect, new Color(0.08f, 0.08f, 0.09f, 1f));

            DrawCanvasAxes(canvasRect);
            DrawConnections(canvasRect);
            DrawNodes(canvasRect);
            HandleCanvasContextMenu(canvasRect);

            if (draggingNode != null && Event.current.rawType == EventType.MouseUp)
            {
                draggingNode = null;
                layoutChanged = true;
            }

            GUI.EndScrollView();

            return layoutChanged;
        }

        private void DrawCanvasAxes(Rect canvasRect)
        {
            Handles.BeginGUI();
            Handles.color = new Color(1f, 1f, 1f, 0.08f);

            Handles.DrawLine(new Vector3(canvasRect.center.x, canvasRect.yMin), new Vector3(canvasRect.center.x, canvasRect.yMax));

            Handles.DrawLine(new Vector3(canvasRect.xMin, canvasRect.center.y), new Vector3(canvasRect.xMax, canvasRect.center.y));

            Handles.EndGUI();
        }

        private void DrawConnections(Rect canvasRect)
        {
            HashSet<CharacterMasteryNodeDefinition> nodes = GetNodeSet();

            Handles.BeginGUI();
            Handles.color = new Color(0.55f, 0.58f, 0.64f, 0.9f);

            foreach (CharacterMasteryNodeDefinition node in Mastery.Nodes)
            {
                if (node == null)
                    continue;

                Vector2 toCenter = TreeToCanvas(node.TreePosition, canvasRect);

                foreach (CharacterMasteryNodeDefinition prerequisite in node.Prerequisites)
                {
                    if (prerequisite == null || !nodes.Contains(prerequisite))
                    {
                        continue;
                    }

                    Vector2 fromCenter = TreeToCanvas(prerequisite.TreePosition, canvasRect);

                    Vector2 delta = toCenter - fromCenter;

                    if (delta.sqrMagnitude <= 0.0001f)
                        continue;

                    Vector2 direction = delta.normalized;

                    Vector2 from = GetBoundaryPoint(fromCenter, NodeSize, direction);

                    Vector2 to = GetBoundaryPoint(toCenter, NodeSize, -direction);

                    Handles.DrawAAPolyLine(3f, from, to);
                }
            }

            Handles.EndGUI();
        }

        private void DrawNodes(Rect canvasRect)
        {
            foreach (CharacterMasteryNodeDefinition node in Mastery.Nodes)
            {
                if (node == null)
                    continue;

                Vector2 center = TreeToCanvas(node.TreePosition, canvasRect);

                Rect nodeRect = new(center - NodeSize * 0.5f, NodeSize);

                Color typeColor = GameContentAuthoringPalette.GetMasteryNodeColor(node);

                GUI.Box(nodeRect, GUIContent.none, EditorStyles.helpBox);

                DrawNodeTypePresentation(nodeRect, typeColor);

                Rect titleRect = new(nodeRect.x + 12f, nodeRect.y + 10f, nodeRect.width - 20f, 24f);

                Rect typeRect = new(nodeRect.x + 12f, nodeRect.y + 48f, nodeRect.width - 20f, 20f);

                GUI.Label(titleRect, node.DisplayName, EditorStyles.boldLabel);

                GUIStyle typeStyle = new(EditorStyles.miniBoldLabel);
                typeStyle.normal.textColor = typeColor;

                GUI.Label(typeRect, CharacterMasteryWorkbenchInspector.GetTypeLabel(node), typeStyle);

                if (selectedNode == node)
                    DrawSelectionOutline(nodeRect);

                HandleNodeInput(node, nodeRect, canvasRect);
            }
        }

        private void HandleNodeInput(CharacterMasteryNodeDefinition node, Rect nodeRect, Rect canvasRect)
        {
            Event current = Event.current;

            if (current.type == EventType.ContextClick && nodeRect.Contains(current.mousePosition))
            {
                GenericMenu menu = new();

                menu.AddItem(
                    new GUIContent("Select Asset"),
                    false,
                    () =>
                    {
                        Selection.activeObject = node;
                        EditorGUIUtility.PingObject(node);
                    }
                );

                menu.ShowAsContext();
                current.Use();
                return;
            }

            if (draggingNode == null && current.type == EventType.MouseDown && current.button == 0 && nodeRect.Contains(current.mousePosition))
            {
                selectedNode = node;
                draggingNode = node;
                dragOffset = current.mousePosition - nodeRect.center;

                Undo.RecordObject(node, "Move Mastery Node");

                if (current.clickCount >= 2)
                {
                    Selection.activeObject = node;
                    EditorGUIUtility.PingObject(node);
                }

                current.Use();
                GUI.changed = true;
                return;
            }

            if (draggingNode != node || current.type != EventType.MouseDrag || current.button != 0)
            {
                return;
            }

            Vector2 canvasPosition = current.mousePosition - dragOffset;

            Vector2 treePosition = CanvasToTree(canvasPosition, canvasRect);

            treePosition = ClampTreePosition(treePosition, Mastery.TreeSize);

            SetTreePosition(node, treePosition);

            current.Use();
            GUI.changed = true;
        }

        private void HandleCanvasContextMenu(Rect canvasRect)
        {
            Event current = Event.current;

            if (current.type != EventType.ContextClick || !canvasRect.Contains(current.mousePosition) || IsPointerOverNode(current.mousePosition, canvasRect))
            {
                return;
            }

            Vector2 treePosition = ClampTreePosition(CanvasToTree(current.mousePosition, canvasRect), Mastery.TreeSize);

            ShowCreateMenu(treePosition);

            current.Use();
        }

        private bool IsPointerOverNode(Vector2 pointer, Rect canvasRect)
        {
            foreach (CharacterMasteryNodeDefinition node in Mastery.Nodes)
            {
                if (node == null)
                    continue;

                Vector2 center = TreeToCanvas(node.TreePosition, canvasRect);

                Rect nodeRect = new(center - NodeSize * 0.5f, NodeSize);

                if (nodeRect.Contains(pointer))
                    return true;
            }

            return false;
        }

        private void ShowCreateMenu(Vector2 position)
        {
            GenericMenu menu = new();

            AddCreateMenuItem(menu, CharacterMasteryNodeKind.Stat, position);

            AddCreateMenuItem(menu, CharacterMasteryNodeKind.AbilityLevel, position);

            AddCreateMenuItem(menu, CharacterMasteryNodeKind.WeaponMastery, position);

            AddCreateMenuItem(menu, CharacterMasteryNodeKind.WeaponMasteryLevel, position);

            AddCreateMenuItem(menu, CharacterMasteryNodeKind.WeaponMasteryCombination, position);

            menu.ShowAsContext();
        }

        private void AddCreateMenuItem(GenericMenu menu, CharacterMasteryNodeKind kind, Vector2 position)
        {
            menu.AddItem(new GUIContent(CharacterMasteryWorkbenchNodeFactory.GetMenuLabel(kind)), false, () => CharacterMasteryNodeCreateWindow.Open(character, Mastery, kind, position));
        }

        private Vector2 FindOpenPosition()
        {
            if (Mastery == null)
                return Vector2.zero;

            Vector2 halfCanvas = Mastery.TreeSize * 0.5f - NodeSize * 0.5f;

            for (int radius = 0; radius < 20; radius++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        if (Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) != radius)
                        {
                            continue;
                        }

                        Vector2 candidate = new(x * GridStepX, y * GridStepY);

                        if (Mathf.Abs(candidate.x) > halfCanvas.x || Mathf.Abs(candidate.y) > halfCanvas.y)
                        {
                            continue;
                        }

                        if (!OverlapsExistingNode(candidate))
                        {
                            return candidate;
                        }
                    }
                }
            }

            return Vector2.zero;
        }

        private bool OverlapsExistingNode(Vector2 position)
        {
            Rect candidate = GetNodeRect(position);

            foreach (CharacterMasteryNodeDefinition node in Mastery.Nodes)
            {
                if (node != null && candidate.Overlaps(GetNodeRect(node.TreePosition)))
                {
                    return true;
                }
            }

            return false;
        }

        private HashSet<CharacterMasteryNodeDefinition> GetNodeSet()
        {
            HashSet<CharacterMasteryNodeDefinition> nodes = new();

            if (Mastery == null)
                return nodes;

            foreach (CharacterMasteryNodeDefinition node in Mastery.Nodes)
            {
                if (node != null)
                    nodes.Add(node);
            }

            return nodes;
        }

        private static void SetTreePosition(CharacterMasteryNodeDefinition node, Vector2 position)
        {
            SerializedObject serialized = new(node);

            SerializedProperty property = serialized.FindProperty("treePosition");

            if (property == null)
                return;

            property.vector2Value = position;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(node);
        }

        private static Vector2 TreeToCanvas(Vector2 position, Rect canvasRect)
        {
            return new Vector2(canvasRect.center.x + position.x, canvasRect.center.y - position.y);
        }

        private static Vector2 CanvasToTree(Vector2 position, Rect canvasRect)
        {
            return new Vector2(position.x - canvasRect.center.x, canvasRect.center.y - position.y);
        }

        private static Vector2 ClampTreePosition(Vector2 position, Vector2 canvasSize)
        {
            Vector2 halfCanvas = canvasSize * 0.5f;

            Vector2 halfNode = NodeSize * 0.5f;

            return new Vector2(Mathf.Clamp(position.x, -halfCanvas.x + halfNode.x, halfCanvas.x - halfNode.x), Mathf.Clamp(position.y, -halfCanvas.y + halfNode.y, halfCanvas.y - halfNode.y));
        }

        private static Rect GetNodeRect(Vector2 position)
        {
            return new Rect(position - NodeSize * 0.5f, NodeSize);
        }

        private static Vector2 GetBoundaryPoint(Vector2 center, Vector2 size, Vector2 direction)
        {
            if (direction.sqrMagnitude <= 0.0001f)
                return center;

            float xDistance = Mathf.Abs(direction.x) > 0.0001f ? size.x * 0.5f / Mathf.Abs(direction.x) : float.PositiveInfinity;

            float yDistance = Mathf.Abs(direction.y) > 0.0001f ? size.y * 0.5f / Mathf.Abs(direction.y) : float.PositiveInfinity;

            return center + direction * Mathf.Min(xDistance, yDistance);
        }

        private static void DrawNodeTypePresentation(Rect rect, Color color)
        {
            Rect accentRect = new(rect.x + 2f, rect.y + 2f, 5f, rect.height - 4f);

            EditorGUI.DrawRect(accentRect, color);

            Rect outlineRect = new(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);

            Handles.BeginGUI();
            Handles.color = color;

            Vector3[] points =
            {
                new(outlineRect.xMin, outlineRect.yMin),
                new(outlineRect.xMax, outlineRect.yMin),
                new(outlineRect.xMax, outlineRect.yMax),
                new(outlineRect.xMin, outlineRect.yMax),
                new(outlineRect.xMin, outlineRect.yMin),
            };

            Handles.DrawAAPolyLine(1.5f, points);
            Handles.EndGUI();
        }

        private static void DrawSelectionOutline(Rect rect)
        {
            Rect selectionRect = new(rect.x - 3f, rect.y - 3f, rect.width + 6f, rect.height + 6f);

            Handles.BeginGUI();
            Handles.color = new Color(0.95f, 0.78f, 0.2f, 1f);

            Vector3[] points =
            {
                new(selectionRect.xMin, selectionRect.yMin),
                new(selectionRect.xMax, selectionRect.yMin),
                new(selectionRect.xMax, selectionRect.yMax),
                new(selectionRect.xMin, selectionRect.yMax),
                new(selectionRect.xMin, selectionRect.yMin),
            };

            Handles.DrawAAPolyLine(3f, points);
            Handles.EndGUI();
        }
    }
}
