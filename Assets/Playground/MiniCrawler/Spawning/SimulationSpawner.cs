using MiniCrawler.Core;
using UnityEngine;

namespace MiniCrawler.Spawning
{
    public class SimulationSpawner : MonoBehaviour
    {
        public static SimulationSpawner Instance { get; private set; }

        [SerializeField] private Transform spawnedObjectsParent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public GameObject Spawn(
            ActorDefinition definition,
            Vector3 position,
            Quaternion rotation,
            GameObject source = null
        )
        {
            if (definition == null)
            {
                Debug.LogError("[Spawner] Cannot spawn. ActorDefinition is null.");
                return null;
            }

            if (definition.Prefab == null)
            {
                Debug.LogError($"[Spawner] Cannot spawn '{definition.name}'. Prefab is missing.");
                return null;
            }

            GameObject spawned = Instantiate(
                definition.Prefab,
                position,
                rotation,
                spawnedObjectsParent
            );

            string sourceName = source != null ? source.name : "Unknown";
            Debug.Log($"[Spawner] Spawned {definition.Id} from {sourceName}.");

            return spawned;
        }
    }
}