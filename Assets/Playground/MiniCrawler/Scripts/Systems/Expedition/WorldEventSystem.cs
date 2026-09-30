using MiniCrawler.Expedition;
using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(-440)]
    [DisallowMultipleComponent]
    public sealed class WorldEventSystem : MonoBehaviour
    {
        public static WorldEventSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Update()
        {
            WorldClockState worldClock = RunProgress.WorldClock;
            WorldEventState worldEvents = RunProgress.WorldEvents;

            if (worldClock == null || worldEvents == null || !worldClock.IsInitialized)
                return;

            worldEvents.ExpireDue(worldClock.CurrentTime);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}