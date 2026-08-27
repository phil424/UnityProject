using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Actor))]
    public class FactionMember : MonoBehaviour
    {
        [SerializeField] private string factionId = "Neutral";

        public string FactionId => factionId;

        public bool IsFaction(string id)
        {
            return factionId == id;
        }

        public bool IsSameFactionAs(FactionMember other)
        {
            if (other == null)
                return false;

            return factionId == other.factionId;
        }
    }
}