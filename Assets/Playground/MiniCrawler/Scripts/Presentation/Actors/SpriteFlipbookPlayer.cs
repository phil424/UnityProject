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
        private float playbackSpeed = 1f;
        private int currentFrameIndex = -1;

        public SpriteFlipbookClip CurrentClip => currentClip;
        public float PlaybackSpeed => playbackSpeed;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (currentClip == null || currentClip.FrameCount <= 0 || Time.deltaTime <= 0f)
                return;

            elapsedSeconds += Time.deltaTime * playbackSpeed;
            ApplyFrame(CalculateFrameIndex());
        }

        public void Play(SpriteFlipbookClip clip, bool restartIfSame = false, float speed = 1f, float normalizedTime = 0f)
        {
            if (clip == null)
            {
                Stop(true);
                return;
            }

            playbackSpeed = Mathf.Max(0f, speed);

            if (!restartIfSame && currentClip == clip)
                return;

            currentClip = clip;
            elapsedSeconds = currentClip.Duration * Mathf.Clamp01(normalizedTime);
            currentFrameIndex = -1;

            ApplyFrame(CalculateFrameIndex());
        }

        public void Stop(bool clearSprite = false)
        {
            currentClip = null;
            elapsedSeconds = 0f;
            playbackSpeed = 1f;
            currentFrameIndex = -1;

            if (clearSprite)
                spriteRenderer.sprite = null;
        }

        private int CalculateFrameIndex()
        {
            if (currentClip == null || currentClip.FrameCount <= 0)
                return -1;

            int frameIndex = Mathf.FloorToInt(elapsedSeconds * currentClip.FramesPerSecond);

            if (currentClip.Loop)
                frameIndex %= currentClip.FrameCount;
            else
                frameIndex = Mathf.Min(frameIndex, currentClip.FrameCount - 1);

            return Mathf.Max(0, frameIndex);
        }

        private void ApplyFrame(int frameIndex)
        {
            if (currentClip == null || frameIndex < 0 || frameIndex == currentFrameIndex)
                return;

            spriteRenderer.sprite = currentClip.GetFrame(frameIndex);
            currentFrameIndex = frameIndex;
        }
    }
}