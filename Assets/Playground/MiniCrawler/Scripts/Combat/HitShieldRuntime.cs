using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniCrawler.Combat
{
    [DisallowMultipleComponent]
    public sealed class HitShieldRuntime : MonoBehaviour
    {
        private sealed class ShieldLayer
        {
            public int HitsRemaining;
        }

        private readonly List<ShieldLayer> shieldLayers = new();

        public event Action<HitShieldRuntime> Changed;

        public int ShieldCount => shieldLayers.Count;

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

        public bool AddShields(GameObject source, int shieldCount, int hitsPerShield, string actionName)
        {
            shieldCount = Mathf.Max(0, shieldCount);
            hitsPerShield = Mathf.Max(0, hitsPerShield);

            if (shieldCount <= 0 || hitsPerShield <= 0)
                return false;

            for (int i = 0; i < shieldCount; i++)
            {
                shieldLayers.Add(new ShieldLayer { HitsRemaining = hitsPerShield });
            }

            Changed?.Invoke(this);

            string sourceName = source != null ? source.name : "Unknown";

            Debug.Log(
                $"[Shield] {name} gained {shieldCount} shield(s) from {sourceName} "
                    + $"[{actionName}] with {hitsPerShield} hit(s) each | "
                    + $"Shields {ShieldCount} | Total Hits {TotalHitsRemaining}."
            );

            return true;
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

            string result = shieldBroken ? "Shield broke" : $"Current Shield {currentShield.HitsRemaining} hit(s) remaining";

            Debug.Log($"[Shield] {name} absorbed {sourceName} [{actionName}] | " + $"{result} | Shields {ShieldCount} | Total Hits {TotalHitsRemaining}.");

            return true;
        }

        private void OnDisable()
        {
            shieldLayers.Clear();
        }
    }
}
