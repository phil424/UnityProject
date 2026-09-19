using UnityEngine;

namespace MiniCrawler.Presentation
{
    [CreateAssetMenu(
        fileName = "New Sprite Flipbook Clip",
        menuName = "Mini Crawler/Presentation/Sprite Flipbook Clip"
    )]
    public class SpriteFlipbookClip : ScriptableObject
    {
        [SerializeField] private Sprite[] frames;
        [SerializeField, Min(0.01f)] private float framesPerSecond = 8f;
        [SerializeField] private bool loop = true;

        public int FrameCount => frames != null ? frames.Length : 0;
        public float FramesPerSecond => framesPerSecond;
        public float Duration => FrameCount <= 0 ? 0f : FrameCount / framesPerSecond;
        public bool Loop => loop;

        public Sprite GetFrame(int index)
        {
            if (frames == null || index < 0 || index >= frames.Length)
                return null;

            return frames[index];
        }

        private void OnValidate()
        {
            framesPerSecond = Mathf.Max(0.01f, framesPerSecond);
        }
    }
}