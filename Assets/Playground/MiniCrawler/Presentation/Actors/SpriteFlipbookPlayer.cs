using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFlipbookPlayer : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private SpriteFlipbookClip currentClip;
        private float elapsedSeconds;
        private int currentFrameIndex = -1;

        public SpriteFlipbookClip CurrentClip => currentClip;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (currentClip == null || currentClip.FrameCount <= 0 || Time.deltaTime <= 0f)
                return;

            elapsedSeconds += Time.deltaTime;

            int frameIndex = Mathf.FloorToInt(elapsedSeconds * currentClip.FramesPerSecond);

            if (currentClip.Loop)
                frameIndex %= currentClip.FrameCount;
            else
                frameIndex = Mathf.Min(frameIndex, currentClip.FrameCount - 1);

            ApplyFrame(frameIndex);
        }

        public void Play(SpriteFlipbookClip clip, bool restartIfSame = false)
        {
            if (clip == null)
            {
                Stop(true);
                return;
            }

            if (!restartIfSame && currentClip == clip)
                return;

            currentClip = clip;
            elapsedSeconds = 0f;
            currentFrameIndex = -1;

            ApplyFrame(0);
        }

        public void Stop(bool clearSprite = false)
        {
            currentClip = null;
            elapsedSeconds = 0f;
            currentFrameIndex = -1;

            if (clearSprite)
                spriteRenderer.sprite = null;
        }

        private void ApplyFrame(int frameIndex)
        {
            if (currentClip == null || frameIndex == currentFrameIndex)
                return;

            spriteRenderer.sprite = currentClip.GetFrame(frameIndex);
            currentFrameIndex = frameIndex;
        }
    }
}