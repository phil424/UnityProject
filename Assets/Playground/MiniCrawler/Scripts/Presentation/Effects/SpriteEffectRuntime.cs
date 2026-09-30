using UnityEngine;
using UnityEngine.Rendering;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    public class SpriteEffectRuntime : MonoBehaviour
    {
        private float remainingLifetime;

        private bool followsTarget;
        private Transform followTarget;
        private Vector3 followOffset;

        public static SpriteEffectRuntime Spawn(
            SpriteEffectDefinition definition,
            Vector3 worldPosition,
            bool mirrorHorizontally = false)
        {
            return Create(
                definition,
                worldPosition,
                definition != null && definition.Clip != null ? definition.Clip.Duration : 0f,
                mirrorHorizontally
            );
        }

        public static SpriteEffectRuntime SpawnAttached(
            SpriteEffectDefinition definition,
            Transform target,
            float lifetime,
            bool mirrorHorizontally = false)
        {
            if (target == null)
                return null;

            SpriteEffectRuntime runtime = Create(
                definition,
                target.position,
                lifetime,
                mirrorHorizontally
            );

            if (runtime == null)
                return null;

            runtime.followsTarget = true;
            runtime.followTarget = target;
            runtime.followOffset = definition.WorldOffset;

            return runtime;
        }

        private static SpriteEffectRuntime Create(
            SpriteEffectDefinition definition,
            Vector3 worldPosition,
            float lifetime,
            bool mirrorHorizontally)
        {
            if (definition == null ||
                definition.Clip == null ||
                definition.Clip.FrameCount <= 0 ||
                lifetime <= 0f)
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

            runtime.remainingLifetime = lifetime;

            flipbookPlayer.Play(definition.Clip, true);

            return runtime;
        }

        private void Update()
        {
            if (followsTarget)
            {
                if (followTarget == null)
                {
                    Destroy(gameObject);
                    return;
                }

                transform.position = followTarget.position + followOffset;
            }

            if (Time.deltaTime <= 0f)
                return;

            remainingLifetime -= Time.deltaTime;

            if (remainingLifetime <= 0f)
                Destroy(gameObject);
        }
    }
}