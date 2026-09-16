using System;
using System.Collections.Generic;
using MiniCrawler.Core;
using MiniCrawler.Movement;
using MiniCrawler.Systems;
using UnityEngine;

namespace MiniCrawler.Ecology
{
    [Serializable]
    public sealed class AmbientPopulationEntry
    {
        [SerializeField] private ActorDefinition actorDefinition;
        [SerializeField, Min(1)] private int count = 1;

        public ActorDefinition ActorDefinition => actorDefinition;
        public int Count => count;
    }

    [DisallowMultipleComponent]
    public sealed class AmbientActivityArea : MonoBehaviour
    {
        [Header("Route Footprint")]
        [SerializeField] private List<AmbientRouteNode> nodes = new();
        [SerializeField] private List<AmbientRouteSegment> segments = new();
        [SerializeField] private List<AmbientRouteGroup> groups = new();

        [Header("Population")]
        [SerializeField] private List<AmbientPopulationEntry> populations = new();

        [Header("Placement")]
        [SerializeField, Min(0f)] private float spawnScatterRadius = 1.5f;

        [Header("Engagement")]
        [SerializeField, Min(0.1f)] private float engagementRadius = 3.5f;

        private readonly List<AmbientRouteSegment> segmentBuffer = new();
        private readonly List<AmbientRouteNode> nodeBuffer = new();

        private StageDirector stageDirector;
        private bool spawnedForLevel;

        public IReadOnlyList<AmbientRouteNode> Nodes => nodes;
        public IReadOnlyList<AmbientRouteSegment> Segments => segments;
        public IReadOnlyList<AmbientRouteGroup> Groups => groups;
        public IReadOnlyList<AmbientPopulationEntry> Populations => populations;

        public float SpawnScatterRadius => spawnScatterRadius;
        public float EngagementRadius => engagementRadius;
        public AmbientRouteGraph RouteGraph => ResolveRouteGraph();

        private void Start()
        {
            EnsureStageDirector();

            if (stageDirector == null)
            {
                Debug.LogWarning("AmbientActivityArea could not find StageDirector.", this);
                return;
            }

            stageDirector.StateChanged += HandleStageStateChanged;
            stageDirector.LevelCleared += HandleLevelCleared;

            if (stageDirector.State == StageDirector.LevelState.FightingMinions)
                SpawnForLevel();
        }

        private void OnDestroy()
        {
            if (stageDirector == null)
                return;

            stageDirector.StateChanged -= HandleStageStateChanged;
            stageDirector.LevelCleared -= HandleLevelCleared;
        }

        public bool SupportsActor(ActorDefinition actorDefinition)
        {
            if (actorDefinition == null)
                return false;

            foreach (AmbientPopulationEntry population in populations)
            {
                if (population != null && population.ActorDefinition == actorDefinition)
                    return true;
            }

            return false;
        }

        public int SpawnAdditional(ActorDefinition actorDefinition, int additionalCount)
        {
            if (actorDefinition == null || additionalCount <= 0 || !SupportsActor(actorDefinition))
                return 0;

            EnsureStageDirector();

            if (stageDirector == null || stageDirector.State != StageDirector.LevelState.FightingMinions)
                return 0;

            return SpawnActors(actorDefinition, additionalCount);
        }

        public bool ReferencesGraph(AmbientRouteGraph graph)
        {
            if (graph == null)
                return false;

            foreach (AmbientRouteNode node in nodes)
            {
                if (node != null && graph.ContainsNode(node))
                    return true;
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment != null && graph.ContainsSegment(segment))
                    return true;
            }

            foreach (AmbientRouteGroup group in groups)
            {
                if (group != null && group.GetComponentInParent<AmbientRouteGraph>() == graph)
                    return true;
            }

            return false;
        }

        public void CollectValidationIssues(AmbientRouteGraph expectedGraph, List<string> issues)
        {
            bool hasFootprintReference = false;
            HashSet<AmbientRouteNode> seenNodes = new();
            HashSet<AmbientRouteSegment> seenSegments = new();
            HashSet<AmbientRouteGroup> seenGroups = new();

            foreach (AmbientRouteNode node in nodes)
            {
                if (node == null)
                {
                    issues.Add($"Ambient Activity Area '{name}' contains an empty Node reference.");
                    continue;
                }

                hasFootprintReference = true;

                if (!seenNodes.Add(node))
                    issues.Add($"Ambient Activity Area '{name}' contains duplicate Node '{node.DisplayName}'.");

                if (expectedGraph != null && !expectedGraph.ContainsNode(node))
                    issues.Add($"Ambient Activity Area '{name}' references Node '{node.DisplayName}' outside the selected Route Graph.");
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment == null)
                {
                    issues.Add($"Ambient Activity Area '{name}' contains an empty Segment reference.");
                    continue;
                }

                hasFootprintReference = true;

                if (!seenSegments.Add(segment))
                    issues.Add($"Ambient Activity Area '{name}' contains duplicate Segment '{segment.DisplayName}'.");

                if (!segment.IsConfigured)
                    issues.Add($"Ambient Activity Area '{name}' uses unconfigured Segment '{segment.DisplayName}'.");

                if (expectedGraph != null && !expectedGraph.ContainsSegment(segment))
                    issues.Add($"Ambient Activity Area '{name}' references Segment '{segment.DisplayName}' outside the selected Route Graph.");
            }

            foreach (AmbientRouteGroup group in groups)
            {
                if (group == null)
                {
                    issues.Add($"Ambient Activity Area '{name}' contains an empty Group reference.");
                    continue;
                }

                hasFootprintReference = true;

                if (!seenGroups.Add(group))
                    issues.Add($"Ambient Activity Area '{name}' contains duplicate Group '{group.DisplayName}'.");

                if (expectedGraph != null && group.GetComponentInParent<AmbientRouteGraph>() != expectedGraph)
                    issues.Add($"Ambient Activity Area '{name}' references Group '{group.DisplayName}' outside the selected Route Graph.");
            }

            if (!hasFootprintReference)
                issues.Add($"Ambient Activity Area '{name}' has no Route footprint.");

            HashSet<ActorDefinition> seenActors = new();

            if (populations.Count == 0)
                issues.Add($"Ambient Activity Area '{name}' has no population entries.");

            foreach (AmbientPopulationEntry population in populations)
            {
                if (population == null)
                {
                    issues.Add($"Ambient Activity Area '{name}' contains an empty population entry.");
                    continue;
                }

                if (population.ActorDefinition == null)
                {
                    issues.Add($"Ambient Activity Area '{name}' contains a population entry with no Actor Definition.");
                    continue;
                }

                if (!seenActors.Add(population.ActorDefinition))
                {
                    issues.Add(
                        $"Ambient Activity Area '{name}' contains duplicate population Actor '{population.ActorDefinition.name}'."
                    );
                }

                if (population.Count <= 0)
                    issues.Add($"Ambient Activity Area '{name}' has a non-positive population count.");
            }

            if (spawnScatterRadius < 0f)
                issues.Add($"Ambient Activity Area '{name}' has a negative Spawn Scatter Radius.");

            if (engagementRadius <= 0f)
                issues.Add($"Ambient Activity Area '{name}' must have a positive Engagement Radius.");

            CollectSpawnSegments(segmentBuffer);
            CollectSpawnNodes(nodeBuffer);

            if (segmentBuffer.Count == 0 && nodeBuffer.Count == 0)
                issues.Add($"Ambient Activity Area '{name}' has no usable Route geography for spawning.");
        }

        private void HandleStageStateChanged(StageDirector.LevelState state)
        {
            if (state == StageDirector.LevelState.FightingMinions)
                SpawnForLevel();
        }

        private void HandleLevelCleared()
        {
            spawnedForLevel = false;
        }

        private void SpawnForLevel()
        {
            if (spawnedForLevel || stageDirector == null)
                return;

            spawnedForLevel = true;

            foreach (AmbientPopulationEntry population in populations)
            {
                if (population?.ActorDefinition == null || population.Count <= 0)
                    continue;

                SpawnActors(population.ActorDefinition, population.Count);
            }
        }

        private int SpawnActors(ActorDefinition actorDefinition, int amount)
        {
            int spawnedCount = 0;

            for (int i = 0; i < amount; i++)
            {
                if (!TryGetSpawnPose(out Pose pose))
                    continue;

                GameObject spawned = stageDirector.SpawnAmbientActor(actorDefinition, pose, engagementRadius);

                if (spawned != null)
                    spawnedCount++;
            }

            return spawnedCount;
        }

        private bool TryGetSpawnPose(out Pose pose)
        {
            CollectSpawnSegments(segmentBuffer);

            Vector3 position;

            if (segmentBuffer.Count > 0)
            {
                AmbientRouteSegment segment = ChooseWeightedSegment(segmentBuffer);
                position = Vector3.Lerp(segment.NodeA.Position, segment.NodeB.Position, UnityEngine.Random.value);
            }
            else
            {
                CollectSpawnNodes(nodeBuffer);

                if (nodeBuffer.Count == 0)
                {
                    pose = default;
                    return false;
                }

                position = nodeBuffer[UnityEngine.Random.Range(0, nodeBuffer.Count)].Position;
            }

            Vector2 scatter = UnityEngine.Random.insideUnitCircle * Mathf.Max(0f, spawnScatterRadius);
            position += new Vector3(scatter.x, 0f, scatter.y);

            Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            pose = new Pose(position, rotation);
            return true;
        }

        private AmbientRouteSegment ChooseWeightedSegment(IReadOnlyList<AmbientRouteSegment> candidates)
        {
            float totalLength = 0f;

            foreach (AmbientRouteSegment segment in candidates)
                totalLength += Mathf.Max(0f, segment.Length);

            if (totalLength <= 0f)
                return candidates[UnityEngine.Random.Range(0, candidates.Count)];

            float remaining = UnityEngine.Random.value * totalLength;

            foreach (AmbientRouteSegment segment in candidates)
            {
                remaining -= Mathf.Max(0f, segment.Length);

                if (remaining <= 0f)
                    return segment;
            }

            return candidates[candidates.Count - 1];
        }

        private void CollectSpawnSegments(List<AmbientRouteSegment> results)
        {
            results.Clear();

            foreach (AmbientRouteSegment segment in segments)
                AddUnique(results, segment);

            foreach (AmbientRouteGroup group in groups)
            {
                if (group == null)
                    continue;

                foreach (AmbientRouteSegment segment in group.Segments)
                    AddUnique(results, segment);
            }

            for (int i = results.Count - 1; i >= 0; i--)
            {
                if (results[i] == null || !results[i].IsConfigured)
                    results.RemoveAt(i);
            }
        }

        private void CollectSpawnNodes(List<AmbientRouteNode> results)
        {
            results.Clear();

            foreach (AmbientRouteNode node in nodes)
                AddUnique(results, node);

            foreach (AmbientRouteGroup group in groups)
            {
                if (group == null)
                    continue;

                foreach (AmbientRouteNode node in group.Nodes)
                    AddUnique(results, node);
            }
        }

        private AmbientRouteGraph ResolveRouteGraph()
        {
            foreach (AmbientRouteNode node in nodes)
            {
                if (node != null)
                    return node.GetComponentInParent<AmbientRouteGraph>();
            }

            foreach (AmbientRouteSegment segment in segments)
            {
                if (segment != null)
                    return segment.GetComponentInParent<AmbientRouteGraph>();
            }

            foreach (AmbientRouteGroup group in groups)
            {
                if (group != null)
                    return group.GetComponentInParent<AmbientRouteGraph>();
            }

            return null;
        }

        private void EnsureStageDirector()
        {
            if (stageDirector == null)
                stageDirector = StageDirector.Instance;
        }

        private static void AddUnique<T>(List<T> items, T item) where T : UnityEngine.Object
        {
            if (item != null && !items.Contains(item))
                items.Add(item);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            CollectSpawnSegments(segmentBuffer);

            foreach (AmbientRouteSegment segment in segmentBuffer)
                Gizmos.DrawLine(segment.NodeA.Position + Vector3.up * 0.2f, segment.NodeB.Position + Vector3.up * 0.2f);

            CollectSpawnNodes(nodeBuffer);

            foreach (AmbientRouteNode node in nodeBuffer)
            {
                if (node != null)
                    Gizmos.DrawWireSphere(node.Position, 0.6f);
            }
        }
    }
}