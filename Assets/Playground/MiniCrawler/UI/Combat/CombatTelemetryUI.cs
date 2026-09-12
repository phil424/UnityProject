using MiniCrawler.Combat;
using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    public class CombatTelemetryUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text telemetryText;

        private void Update()
        {
            if (telemetryText == null)
                return;

            CombatTelemetry telemetry =
                CombatTelemetry.Instance;

            if (telemetry == null)
            {
                telemetryText.text =
                    "PARTY DPS      --\n" +
                    "INCOMING DPS   --";

                return;
            }

            telemetryText.text =
                $"PARTY DPS      " +
                $"{telemetry.OutgoingDps:0.0}\n" +
                $"INCOMING DPS   " +
                $"{telemetry.IncomingDps:0.0}";
        }
    }
}