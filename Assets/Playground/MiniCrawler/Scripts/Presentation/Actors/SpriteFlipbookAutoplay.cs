using UnityEngine;

namespace MiniCrawler.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteFlipbookPlayer))]
    public class SpriteFlipbookAutoplay : MonoBehaviour
    {
        [SerializeField] private SpriteFlipbookClip clip;

        private SpriteFlipbookPlayer flipbookPlayer;

        private void Awake()
        {
            flipbookPlayer = GetComponent<SpriteFlipbookPlayer>();
        }

        private void OnEnable()
        {
            if (flipbookPlayer == null)
                flipbookPlayer = GetComponent<SpriteFlipbookPlayer>();

            if (clip != null)
                flipbookPlayer.Play(clip, true);
        }
    }
}