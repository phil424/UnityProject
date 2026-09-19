using UnityEngine;
using UnityEngine.Rendering;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    public class SpriteEffectRuntime : MonoBehaviour
    {
        private float remainingLifetime;

        public static SpriteEffectRuntime Spawn(
            SpriteEffectDefinition definition,
            Vector3 worldPosition,
            bool mirrorHorizontally = false
        )
        {
            if (definition == null ||
                definition.Clip == null ||
                definition.Clip.FrameCount <= 0)
            {
                return null;
            }

            GameObject effectObject = new GameObject($"FX - {definition.name}");

            effectObject.transform.position = worldPosition + definition.WorldOffset;

            float xScale = definition.WorldScale * (mirrorHorizontally ? -1f : 1f);
            effectObject.transform.localScale = new Vector3(xScale, definition.WorldScale, definition.WorldScale);

            Camera camera = Camera.main;

            if (camera != null)
            {
                effectObject.transform.rotation =
                    Quaternion.LookRotation(-camera.transform.forward, camera.transform.up);
            }

            SpriteRenderer spriteRenderer = effectObject.AddComponent<SpriteRenderer>();

            if (definition.Material != null)
                spriteRenderer.sharedMaterial = definition.Material;

            spriteRenderer.sortingOrder = definition.SortingOrder;
            spriteRenderer.shadowCastingMode = ShadowCastingMode.Off;
            spriteRenderer.receiveShadows = false;

            SpriteFlipbookPlayer flipbookPlayer = effectObject.AddComponent<SpriteFlipbookPlayer>();
            SpriteEffectRuntime runtime = effectObject.AddComponent<SpriteEffectRuntime>();

            runtime.remainingLifetime = definition.Clip.Duration;
            flipbookPlayer.Play(definition.Clip, true);

            return runtime;
        }

        private void Update()
        {
            if (Time.deltaTime <= 0f)
                return;

            remainingLifetime -= Time.deltaTime;

            if (remainingLifetime <= 0f)
                Destroy(gameObject);
        }
    }
}