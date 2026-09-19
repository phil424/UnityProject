using UnityEngine;

namespace MiniCrawler.Presentation
{
    [CreateAssetMenu(fileName = "New Sprite Effect", menuName = "Mini Crawler/Presentation/Sprite Effect")]
    public class SpriteEffectDefinition : ScriptableObject
    {
        [Header("Visual")]
        [SerializeField] private SpriteFlipbookClip clip;
        [SerializeField] private Material material;

        [Header("Placement")]
        [SerializeField] private Vector3 worldOffset;
        [SerializeField, Min(0.01f)] private float worldScale = 1f;
        [SerializeField] private int sortingOrder = 20;

        [Header("Facing")]
        [SerializeField] private bool sourceFacesRight = true;

        public SpriteFlipbookClip Clip => clip;
        public Material Material => material;
        public Vector3 WorldOffset => worldOffset;
        public float WorldScale => worldScale;
        public int SortingOrder => sortingOrder;
        public bool SourceFacesRight => sourceFacesRight;
    }
}