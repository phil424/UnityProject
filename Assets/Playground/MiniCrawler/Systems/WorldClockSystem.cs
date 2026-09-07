using MiniCrawler.Progress;
using UnityEngine;

namespace MiniCrawler.Systems
{
    [DefaultExecutionOrder(-500)]
    [DisallowMultipleComponent]
    public sealed class WorldClockSystem : MonoBehaviour
    {
        public static WorldClockSystem Instance { get; private set; }

        [Header("Prototype World Clock")]
        [SerializeField, Range(0f, 23.99f)] private float startingHour = 8f;
        [SerializeField, Min(0.01f)] private float worldHoursPerSimulationMinute = 1f;

        public float StartingHour => startingHour;
        public float WorldHoursPerSimulationMinute => worldHoursPerSimulationMinute;
        public float SimulationMinutesPerWorldDay => 24f / worldHoursPerSimulationMinute;

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
            RunState runState = RunProgress.CurrentRun;

            if (runState == null)
                return;

            if (!runState.WorldClock.IsInitialized)
                runState.WorldClock.Initialize(startingHour);

            RunDirector runDirector = RunDirector.Instance;

            if (runDirector == null || runDirector.State != RunDirector.RunFlowState.InLevel)
                return;

            runState.WorldClock.Advance(Time.deltaTime, worldHoursPerSimulationMinute);
        }

        public void SetWorldHoursPerSimulationMinute(float value)
        {
            worldHoursPerSimulationMinute = Mathf.Max(0.01f, value);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void OnValidate()
        {
            startingHour = Mathf.Clamp(startingHour, 0f, 23.99f);
            worldHoursPerSimulationMinute = Mathf.Max(0.01f, worldHoursPerSimulationMinute);
        }
    }
}