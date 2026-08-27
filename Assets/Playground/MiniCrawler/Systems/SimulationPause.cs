using System;
using UnityEngine;

namespace MiniCrawler.Systems
{
    public enum SimulationSpeed
    {
        Paused,
        Slow,
        Normal,
        Fast
    }

    [DisallowMultipleComponent]
    public class SimulationPause : MonoBehaviour
    {
        public static SimulationPause Instance { get; private set; }

        [Header("Time Scales")]
        [SerializeField] private float slowTimeScale = 0.5f;
        [SerializeField] private float fastTimeScale = 2f;

        public static bool IsPaused =>
            Instance != null &&
            Instance.currentSpeed == SimulationSpeed.Paused;

        public event Action<SimulationSpeed> SpeedChanged;

        // Kept for compatibility with anything still interested only in pause state.
        public event Action<bool> PauseChanged;

        private SimulationSpeed currentSpeed = SimulationSpeed.Normal;
        private SimulationSpeed lastRunningSpeed = SimulationSpeed.Normal;

        public SimulationSpeed CurrentSpeed => currentSpeed;
        public bool Paused => currentSpeed == SimulationSpeed.Paused;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            currentSpeed = SimulationSpeed.Normal;
            lastRunningSpeed = SimulationSpeed.Normal;

            ApplyTimeScale();
        }

        public void SetSpeed(SimulationSpeed speed)
        {
            if (currentSpeed == speed)
                return;

            bool wasPaused = Paused;

            if (speed != SimulationSpeed.Paused)
                lastRunningSpeed = speed;

            currentSpeed = speed;

            ApplyTimeScale();

            SpeedChanged?.Invoke(currentSpeed);

            if (wasPaused != Paused)
                PauseChanged?.Invoke(Paused);

            Debug.Log(
                $"[SimulationPause] Speed: {currentSpeed} " +
                $"({Time.timeScale:0.##}x)"
            );
        }

        public void Pause()
        {
            SetSpeed(SimulationSpeed.Paused);
        }

        public void Resume()
        {
            if (!Paused)
                return;

            SetSpeed(lastRunningSpeed);
        }

        public void TogglePause()
        {
            if (Paused)
                Resume();
            else
                Pause();
        }

        private void ApplyTimeScale()
        {
            Time.timeScale = currentSpeed switch
            {
                SimulationSpeed.Paused => 0f,
                SimulationSpeed.Slow => slowTimeScale,
                SimulationSpeed.Fast => fastTimeScale,
                _ => 1f
            };
        }

        private void OnDestroy()
        {
            if (Instance != this)
                return;

            Time.timeScale = 1f;
            Instance = null;
        }

        private void OnValidate()
        {
            slowTimeScale = Mathf.Clamp(slowTimeScale, 0.01f, 0.99f);
            fastTimeScale = Mathf.Max(1.01f, fastTimeScale);
        }
    }
}