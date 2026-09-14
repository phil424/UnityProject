using UnityEngine;

namespace MiniCrawler.Movement
{
    [DisallowMultipleComponent]
    public sealed class AmbientRouteNode : MonoBehaviour
    {
        public string DisplayName => gameObject.name;
        public Vector3 Position => transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, 0.35f);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.55f,
                DisplayName
            );
        }
#endif
    }
}