using MiniCrawler.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    [RequireComponent(typeof(Button))]
    public class SimulationSpeedButtonUI : MonoBehaviour
    {
        [Header("Speed")]
        [SerializeField] private SimulationSpeed speed = SimulationSpeed.Normal;

        [Header("Display")]
        [SerializeField] private TMP_Text buttonText;
        [SerializeField] private string label = "Normal";

        private Button button;
        private SimulationPause simulationPause;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (buttonText == null)
                buttonText = GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
                buttonText.text = label;
        }

        private void OnEnable()
        {
            Bind();
            Refresh();
        }

        private void Start()
        {
            // Handles Unity object initialization order if SimulationPause
            // was not available yet during OnEnable.
            Bind();
            Refresh();
        }

        private void OnDisable()
        {
            Unbind();
        }

        private void Bind()
        {
            SimulationPause current = SimulationPause.Instance;

            if (simulationPause == current)
                return;

            Unbind();

            simulationPause = current;

            if (simulationPause != null)
                simulationPause.SpeedChanged += HandleSpeedChanged;

            button.onClick.AddListener(HandleClicked);
        }

        private void Unbind()
        {
            button.onClick.RemoveListener(HandleClicked);

            if (simulationPause != null)
                simulationPause.SpeedChanged -= HandleSpeedChanged;

            simulationPause = null;
        }

        private void HandleClicked()
        {
            if (simulationPause == null)
                Bind();

            simulationPause?.SetSpeed(speed);
        }

        private void HandleSpeedChanged(SimulationSpeed newSpeed)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (buttonText != null)
                buttonText.text = label;

            if (button == null)
                return;

            button.interactable =
                simulationPause == null ||
                simulationPause.CurrentSpeed != speed;
        }
    }
}