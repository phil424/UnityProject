using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Combat
{
    public readonly struct ShieldBreakEvent
    {
        public ShieldBreakEvent(GameObject shieldSource, string shieldSourceId, string shieldActionName, GameObject impactSource, string impactActionName)
        {
            ShieldSource = shieldSource;
            ShieldSourceId = shieldSourceId;
            ShieldActionName = shieldActionName;
            ImpactSource = impactSource;
            ImpactActionName = impactActionName;
        }

        public GameObject ShieldSource { get; }
        public string ShieldSourceId { get; }
        public string ShieldActionName { get; }

        public GameObject ImpactSource { get; }
        public string ImpactActionName { get; }
    }

    [DisallowMultipleComponent]
    public sealed class HitShieldRuntime : MonoBehaviour
    {
        private sealed class ShieldLayer
        {
            public GameObject Source;
            public string SourceId;
            public string ActionName;
            public int HitsRemaining;
        }

        private readonly List<ShieldLayer> shieldLayers = new();

        private ShieldCapacityStats capacityStats;

        public event Action<HitShieldRuntime> Changed;
        public event Action<ShieldBreakEvent> ShieldBroken;

        public int ShieldCount => shieldLayers.Count;

        public int Capacity => capacityStats != null ? capacityStats.Capacity : int.MaxValue;

        public int AvailableCapacity => capacityStats != null ? Mathf.Max(0, Capacity - ShieldCount) : int.MaxValue;

        public int HitsRemainingInCurrentShield => shieldLayers.Count > 0 ? shieldLayers[0].HitsRemaining : 0;

        public int TotalHitsRemaining
        {
            get
            {
                int total = 0;

                foreach (ShieldLayer layer in shieldLayers)
                    total += layer.HitsRemaining;

                return total;
            }
        }

        public bool HasShields => shieldLayers.Count > 0;

        private void OnEnable()
        {
            BindCapacity();
        }

        public bool AddShields(GameObject source, int shieldCount, int hitsPerShield, string actionName, string sourceId = null)
        {
            shieldCount = Mathf.Max(0, shieldCount);

            hitsPerShield = Mathf.Max(0, hitsPerShield);

            if (shieldCount <= 0 || hitsPerShield <= 0)
            {
                return false;
            }

            int allowedCount = capacityStats != null ? Mathf.Min(shieldCount, AvailableCapacity) : shieldCount;

            if (allowedCount <= 0)
                return false;

            string resolvedActionName = string.IsNullOrWhiteSpace(actionName) ? "Shield" : actionName;

            for (int i = 0; i < allowedCount; i++)
            {
                shieldLayers.Add(
                    new ShieldLayer
                    {
                        Source = source,
                        SourceId = sourceId,
                        ActionName = resolvedActionName,
                        HitsRemaining = hitsPerShield,
                    }
                );
            }

            Changed?.Invoke(this);

            string sourceName = source != null ? source.name : "Unknown";

            string capacityText = capacityStats != null ? $"{ShieldCount}/{Capacity}" : ShieldCount.ToString();

            Debug.Log(
                $"[Shield] {name} gained "
                    + $"{allowedCount} shield(s) from "
                    + $"{sourceName} [{resolvedActionName}] "
                    + $"with {hitsPerShield} hit(s) each | "
                    + $"Shields {capacityText} | "
                    + $"Total Hits {TotalHitsRemaining}."
            );

            return true;
        }

        public int RemoveShieldsBySource(string sourceId)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                return 0;

            int removed = shieldLayers.RemoveAll(layer => layer.SourceId == sourceId);

            if (removed > 0)
                Changed?.Invoke(this);

            return removed;
        }

        public bool TryAbsorbHit(GameObject source, string actionName)
        {
            if (shieldLayers.Count == 0)
                return false;

            ShieldLayer currentShield = shieldLayers[0];

            currentShield.HitsRemaining = Mathf.Max(0, currentShield.HitsRemaining - 1);

            bool shieldBroken = currentShield.HitsRemaining <= 0;

            if (shieldBroken)
                shieldLayers.RemoveAt(0);

            Changed?.Invoke(this);

            string sourceName = source != null ? source.name : "Unknown";

            string result = shieldBroken ? "Shield broke" : $"Current Shield " + $"{currentShield.HitsRemaining} " + $"hit(s) remaining";

            string capacityText = capacityStats != null ? $"{ShieldCount}/{Capacity}" : ShieldCount.ToString();

            Debug.Log($"[Shield] {name} absorbed " + $"{sourceName} [{actionName}] | " + $"{result} | " + $"Shields {capacityText} | " + $"Total Hits {TotalHitsRemaining}.");

            if (shieldBroken)
            {
                ShieldBroken?.Invoke(new ShieldBreakEvent(currentShield.Source, currentShield.SourceId, currentShield.ActionName, source, actionName));
            }

            return true;
        }

        private void BindCapacity()
        {
            if (capacityStats != null)
            {
                capacityStats.Changed -= HandleCapacityChanged;
            }

            capacityStats = GetComponent<ShieldCapacityStats>();

            if (capacityStats != null)
            {
                capacityStats.Changed += HandleCapacityChanged;
            }
        }

        private void HandleCapacityChanged(ShieldCapacityStats changedStats)
        {
            Changed?.Invoke(this);
        }

        private void OnDisable()
        {
            if (capacityStats != null)
            {
                capacityStats.Changed -= HandleCapacityChanged;
            }

            capacityStats = null;
            shieldLayers.Clear();
        }
    }
}
