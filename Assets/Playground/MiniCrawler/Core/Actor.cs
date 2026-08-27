using UnityEngine;

namespace MiniCrawler.Core
{
    [DisallowMultipleComponent]
    public class Actor : MonoBehaviour
    {
        [SerializeField] private string actorName;

        public string ActorName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(actorName))
                    return gameObject.name;

                return actorName;
            }
        }

        private void Reset()
        {
            actorName = gameObject.name;
        }
    }
}