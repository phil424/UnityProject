using System.Collections.Generic;
using MiniCrawler.Movement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MiniCrawler.Tools.RouteAuthoring
{
    internal static class AmbientRouteAuthoringUtility
    {
        private const string NodesRootName = "Nodes";
        private const string SegmentsRootName = "Segments";
        private const string GroupsRootName = "Groups";
        private const string LandmarksRootName = "Landmarks";

        public static AmbientRouteGraph ResolveGraphFromSelection()
        {
            GameObject active = Selection.activeGameObject;

            if (active == null)
                return null;

            AmbientRouteGraph graph = active.GetComponent<AmbientRouteGraph>();

            if (graph != null)
                return graph;

            return active.GetComponentInParent<AmbientRouteGraph>();
        }

        public static void EnsureAuthoringStructure(AmbientRouteGraph graph)
        {
            if (graph == null)
                return;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Ensure Route Authoring Structure");

            bool changed = false;

            EnsureRoot(graph, NodesRootName, ref changed);
            EnsureRoot(graph, SegmentsRootName, ref changed);
            EnsureRoot(graph, GroupsRootName, ref changed);
            EnsureRoot(graph, LandmarksRootName, ref changed);

            if (changed)
                MarkSceneDirty(graph);

            Undo.CollapseUndoOperations(undoGroup);
        }

        public static AmbientRouteNode CreateNode(AmbientRouteGraph graph, string requestedName)
        {
            if (graph == null)
                return null;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create Route Node");

            bool structureChanged = false;
            Transform root = EnsureRoot(graph, NodesRootName, ref structureChanged);

            string baseName = string.IsNullOrWhiteSpace(requestedName)
                ? "Route Node"
                : requestedName.Trim();

            GameObject nodeObject = new(GetUniqueChildName(root, baseName));
            Undo.RegisterCreatedObjectUndo(nodeObject, "Create Route Node");
            Undo.SetTransformParent(nodeObject.transform, root, "Parent Route Node");

            nodeObject.transform.position = GetNewNodePosition(graph);
            nodeObject.transform.rotation = Quaternion.identity;
            nodeObject.transform.localScale = Vector3.one;

            AmbientRouteNode node = Undo.AddComponent<AmbientRouteNode>(nodeObject);

            Selection.activeGameObject = nodeObject;

            MarkSceneDirty(graph);
            SceneView.RepaintAll();

            Undo.CollapseUndoOperations(undoGroup);

            return node;
        }

        public static List<AmbientRouteNode> GetSelectedNodes(AmbientRouteGraph graph)
        {
            List<AmbientRouteNode> result = new();

            if (graph == null)
                return result;

            foreach (GameObject selected in Selection.gameObjects)
            {
                if (selected == null)
                    continue;

                AmbientRouteNode node = selected.GetComponent<AmbientRouteNode>();

                if (node != null &&
                    graph.ContainsNode(node) &&
                    !result.Contains(node))
                {
                    result.Add(node);
                }
            }

            return result;
        }

        public static List<AmbientRouteSegment> GetSelectedSegments(AmbientRouteGraph graph)
        {
            List<AmbientRouteSegment> result = new();

            if (graph == null)
                return result;

            foreach (GameObject selected in Selection.gameObjects)
            {
                if (selected == null)
                    continue;

                AmbientRouteSegment segment = selected.GetComponent<AmbientRouteSegment>();

                if (segment != null &&
                    graph.ContainsSegment(segment) &&
                    !result.Contains(segment))
                {
                    result.Add(segment);
                }
            }

            return result;
        }

        public static List<AmbientRouteSegment> ConnectSelectedPair(AmbientRouteGraph graph)
        {
            List<AmbientRouteNode> nodes = GetSelectedNodes(graph);

            if (nodes.Count != 2)
                return new List<AmbientRouteSegment>();

            return ConnectOrderedNodes(graph, nodes, closeLoop: false);
        }

        public static List<AmbientRouteSegment> ConnectSelectionAsHierarchyPath(AmbientRouteGraph graph)
        {
            List<AmbientRouteNode> nodes = GetSelectedNodes(graph);

            if (nodes.Count < 2)
                return new List<AmbientRouteSegment>();

            SortNodesByHierarchy(nodes);

            return ConnectOrderedNodes(graph, nodes, closeLoop: false);
        }

        public static List<AmbientRouteSegment> ConnectSelectionAsSpatialLoop(AmbientRouteGraph graph)
        {
            List<AmbientRouteNode> nodes = GetSelectedNodes(graph);

            if (nodes.Count < 3)
                return new List<AmbientRouteSegment>();

            SortNodesAroundCentroid(nodes);

            return ConnectOrderedNodes(graph, nodes, closeLoop: true);
        }

        public static AmbientRouteGroup CreateGroupFromSelection(
            AmbientRouteGraph graph,
            string requestedName)
        {
            if (graph == null)
                return null;

            List<AmbientRouteNode> nodes = GetSelectedNodes(graph);
            List<AmbientRouteSegment> segments = GetSelectedSegments(graph);

            if (segments.Count > 0)
            {
                foreach (AmbientRouteSegment segment in segments)
                {
                    AddUnique(nodes, segment.NodeA);
                    AddUnique(nodes, segment.NodeB);
                }
            }
            else if (nodes.Count > 0)
            {
                foreach (AmbientRouteSegment segment in GetOwnedSegments(graph))
                {
                    if (segment == null || !segment.IsConfigured)
                        continue;

                    if (nodes.Contains(segment.NodeA) && nodes.Contains(segment.NodeB))
                        AddUnique(segments, segment);
                }
            }

            if (nodes.Count == 0 && segments.Count == 0)
                return null;

            SortNodesByHierarchy(nodes);
            SortSegmentsByHierarchy(segments);

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Create Route Group");

            bool structureChanged = false;
            Transform root = EnsureRoot(graph, GroupsRootName, ref structureChanged);

            string baseName = string.IsNullOrWhiteSpace(requestedName)
                ? "Route Group"
                : requestedName.Trim();

            GameObject groupObject = new(GetUniqueChildName(root, baseName));
            Undo.RegisterCreatedObjectUndo(groupObject, "Create Route Group");
            Undo.SetTransformParent(groupObject.transform, root, "Parent Route Group");

            groupObject.transform.localPosition = Vector3.zero;
            groupObject.transform.localRotation = Quaternion.identity;
            groupObject.transform.localScale = Vector3.one;

            AmbientRouteGroup group = Undo.AddComponent<AmbientRouteGroup>(groupObject);

            Undo.RecordObject(group, "Configure Route Group");

            SerializedObject serializedGroup = new(group);
            serializedGroup.Update();

            SetReferenceArray(serializedGroup.FindProperty("nodes"), nodes);
            SetReferenceArray(serializedGroup.FindProperty("segments"), segments);

            serializedGroup.ApplyModifiedProperties();

            EditorUtility.SetDirty(group);
            MarkSceneDirty(graph);

            Selection.activeGameObject = groupObject;
            SceneView.RepaintAll();

            Undo.CollapseUndoOperations(undoGroup);

            return group;
        }

        public static List<AmbientRouteNode> GetOwnedNodes(AmbientRouteGraph graph)
        {
            List<AmbientRouteNode> result = new();

            if (graph == null)
                return result;

            foreach (AmbientRouteNode node in graph.GetComponentsInChildren<AmbientRouteNode>(true))
            {
                if (graph.ContainsNode(node))
                    result.Add(node);
            }

            return result;
        }

        public static List<AmbientRouteSegment> GetOwnedSegments(AmbientRouteGraph graph)
        {
            List<AmbientRouteSegment> result = new();

            if (graph == null)
                return result;

            foreach (AmbientRouteSegment segment in graph.GetComponentsInChildren<AmbientRouteSegment>(true))
            {
                if (graph.ContainsSegment(segment))
                    result.Add(segment);
            }

            return result;
        }

        public static List<AmbientRouteGroup> GetOwnedGroups(AmbientRouteGraph graph)
        {
            List<AmbientRouteGroup> result = new();

            if (graph == null)
                return result;

            foreach (AmbientRouteGroup group in graph.GetComponentsInChildren<AmbientRouteGroup>(true))
            {
                if (group != null &&
                    group.GetComponentInParent<AmbientRouteGraph>() == graph)
                {
                    result.Add(group);
                }
            }

            return result;
        }

        public static void LogValidation(AmbientRouteGraph graph)
        {
            if (graph == null)
                return;

            List<string> issues = new();

            if (graph.CollectValidationIssues(issues))
            {
                Debug.Log(
                    $"Route graph '{graph.name}' is valid: " +
                    $"{graph.Nodes.Count} nodes, " +
                    $"{graph.Segments.Count} segments, " +
                    $"{graph.Groups.Count} groups.",
                    graph
                );

                return;
            }

            foreach (string issue in issues)
                Debug.LogWarning($"[Route Geography] {issue}", graph);
        }

        private static List<AmbientRouteSegment> ConnectOrderedNodes(
            AmbientRouteGraph graph,
            IReadOnlyList<AmbientRouteNode> nodes,
            bool closeLoop)
        {
            List<AmbientRouteSegment> result = new();

            if (graph == null || nodes == null || nodes.Count < 2)
                return result;

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName(closeLoop
                ? "Connect Route Nodes As Loop"
                : "Connect Route Nodes As Path");

            bool changed = false;
            bool structureChanged = false;
            Transform root = EnsureRoot(graph, SegmentsRootName, ref structureChanged);

            int connectionCount = closeLoop ? nodes.Count : nodes.Count - 1;

            for (int i = 0; i < connectionCount; i++)
            {
                AmbientRouteNode a = nodes[i];
                AmbientRouteNode b = nodes[(i + 1) % nodes.Count];

                if (a == null || b == null || a == b)
                    continue;

                AmbientRouteSegment segment = FindExistingSegment(graph, a, b);

                if (segment == null)
                {
                    segment = CreateSegment(root, a, b);
                    changed = true;
                }

                AddUnique(result, segment);
            }

            if (changed || structureChanged)
                MarkSceneDirty(graph);

            SelectSegments(result);
            SceneView.RepaintAll();

            Undo.CollapseUndoOperations(undoGroup);

            return result;
        }

        private static AmbientRouteSegment CreateSegment(
            Transform root,
            AmbientRouteNode a,
            AmbientRouteNode b)
        {
            string name = GetUniqueChildName(root, $"{a.DisplayName} to {b.DisplayName}");

            GameObject segmentObject = new(name);
            Undo.RegisterCreatedObjectUndo(segmentObject, "Create Route Segment");
            Undo.SetTransformParent(segmentObject.transform, root, "Parent Route Segment");

            segmentObject.transform.localPosition = Vector3.zero;
            segmentObject.transform.localRotation = Quaternion.identity;
            segmentObject.transform.localScale = Vector3.one;

            AmbientRouteSegment segment = Undo.AddComponent<AmbientRouteSegment>(segmentObject);

            Undo.RecordObject(segment, "Configure Route Segment");

            SerializedObject serializedSegment = new(segment);
            serializedSegment.Update();
            serializedSegment.FindProperty("nodeA").objectReferenceValue = a;
            serializedSegment.FindProperty("nodeB").objectReferenceValue = b;
            serializedSegment.ApplyModifiedProperties();

            EditorUtility.SetDirty(segment);

            return segment;
        }

        private static AmbientRouteSegment FindExistingSegment(
            AmbientRouteGraph graph,
            AmbientRouteNode a,
            AmbientRouteNode b)
        {
            foreach (AmbientRouteSegment segment in GetOwnedSegments(graph))
            {
                if (segment == null || !segment.IsConfigured)
                    continue;

                if ((segment.NodeA == a && segment.NodeB == b) ||
                    (segment.NodeA == b && segment.NodeB == a))
                {
                    return segment;
                }
            }

            return null;
        }

        private static Transform EnsureRoot(
            AmbientRouteGraph graph,
            string rootName,
            ref bool changed)
        {
            Transform existing = FindDirectChild(graph.transform, rootName);

            if (existing != null)
                return existing;

            GameObject rootObject = new(rootName);
            Undo.RegisterCreatedObjectUndo(rootObject, $"Create {rootName} Root");
            Undo.SetTransformParent(rootObject.transform, graph.transform, $"Parent {rootName} Root");

            rootObject.transform.localPosition = Vector3.zero;
            rootObject.transform.localRotation = Quaternion.identity;
            rootObject.transform.localScale = Vector3.one;

            changed = true;

            return rootObject.transform;
        }

        private static Transform FindDirectChild(Transform parent, string childName)
        {
            if (parent == null)
                return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);

                if (child.name == childName)
                    return child;
            }

            return null;
        }

        private static string GetUniqueChildName(Transform parent, string requestedName)
        {
            string baseName = string.IsNullOrWhiteSpace(requestedName)
                ? "Route Item"
                : requestedName.Trim();

            if (FindDirectChild(parent, baseName) == null)
                return baseName;

            for (int i = 2; i < 1000; i++)
            {
                string candidate = $"{baseName} {i:00}";

                if (FindDirectChild(parent, candidate) == null)
                    return candidate;
            }

            return $"{baseName} {System.Guid.NewGuid():N}";
        }

        private static Vector3 GetNewNodePosition(AmbientRouteGraph graph)
        {
            Vector3 position = graph.transform.position;

            if (SceneView.lastActiveSceneView != null)
                position = SceneView.lastActiveSceneView.pivot;

            List<AmbientRouteNode> nodes = GetOwnedNodes(graph);

            if (nodes.Count == 0)
                return position;

            float totalY = 0f;

            foreach (AmbientRouteNode node in nodes)
                totalY += node.Position.y;

            position.y = totalY / nodes.Count;

            return position;
        }

        private static void SortNodesByHierarchy(List<AmbientRouteNode> nodes)
        {
            nodes.Sort((a, b) =>
            {
                if (a == null)
                    return b == null ? 0 : 1;

                if (b == null)
                    return -1;

                if (a.transform.parent == b.transform.parent)
                    return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());

                return string.CompareOrdinal(GetHierarchyPath(a.transform), GetHierarchyPath(b.transform));
            });
        }

        private static void SortSegmentsByHierarchy(List<AmbientRouteSegment> segments)
        {
            segments.Sort((a, b) =>
            {
                if (a == null)
                    return b == null ? 0 : 1;

                if (b == null)
                    return -1;

                if (a.transform.parent == b.transform.parent)
                    return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());

                return string.CompareOrdinal(GetHierarchyPath(a.transform), GetHierarchyPath(b.transform));
            });
        }

        private static void SortNodesAroundCentroid(List<AmbientRouteNode> nodes)
        {
            Vector3 centroid = Vector3.zero;

            foreach (AmbientRouteNode node in nodes)
                centroid += node.Position;

            centroid /= nodes.Count;

            nodes.Sort((a, b) =>
            {
                Vector3 aOffset = a.Position - centroid;
                Vector3 bOffset = b.Position - centroid;

                float aAngle = Mathf.Atan2(aOffset.z, aOffset.x);
                float bAngle = Mathf.Atan2(bOffset.z, bOffset.x);

                int angleComparison = aAngle.CompareTo(bAngle);

                if (angleComparison != 0)
                    return angleComparison;

                return aOffset.sqrMagnitude.CompareTo(bOffset.sqrMagnitude);
            });
        }

        private static string GetHierarchyPath(Transform transform)
        {
            if (transform == null)
                return string.Empty;

            string path = $"{transform.GetSiblingIndex():D4}:{transform.name}";
            Transform current = transform.parent;

            while (current != null)
            {
                path = $"{current.GetSiblingIndex():D4}:{current.name}/{path}";
                current = current.parent;
            }

            return path;
        }

        private static void SelectSegments(IReadOnlyList<AmbientRouteSegment> segments)
        {
            UnityEngine.Object[] selection = new UnityEngine.Object[segments.Count];

            for (int i = 0; i < segments.Count; i++)
                selection[i] = segments[i] != null ? segments[i].gameObject : null;

            Selection.objects = selection;
        }

        private static void SetReferenceArray<T>(
            SerializedProperty property,
            IReadOnlyList<T> items)
            where T : UnityEngine.Object
        {
            property.arraySize = items.Count;

            for (int i = 0; i < items.Count; i++)
                property.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
        }

        private static void AddUnique<T>(List<T> items, T item)
        {
            if (item != null && !items.Contains(item))
                items.Add(item);
        }

        private static void MarkSceneDirty(AmbientRouteGraph graph)
        {
            if (graph != null && graph.gameObject.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(graph.gameObject.scene);
        }
    }
}