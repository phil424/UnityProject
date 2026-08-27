using TMPro;
using UnityEngine;

namespace MiniCrawler.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class FloatingDamageText : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private float riseSpeed = 1f;

        private TMP_Text damageText;
        private Camera targetCamera;
        private float remainingLifetime;

        private void Awake()
        {
            damageText = GetComponent<TMP_Text>();
        }

        public void Initialize(float damage, Camera camera)
        {
            damageText.text = damage.ToString("0.##");

            targetCamera = camera;
            remainingLifetime = lifetime;

            FaceCamera();
        }

        private void Update()
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;

            remainingLifetime -= Time.deltaTime;

            FaceCamera();

            if (remainingLifetime <= 0f)
                Destroy(gameObject);
        }

        private void FaceCamera()
        {
            if (targetCamera == null)
                return;

            transform.rotation = targetCamera.transform.rotation;
        }

        private void OnValidate()
        {
            lifetime = Mathf.Max(0.05f, lifetime);
            riseSpeed = Mathf.Max(0f, riseSpeed);
        }
    }
}