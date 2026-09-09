using UnityEngine;
using UnityEngine.UI;

namespace MiniCrawler.UI
{
    public enum QuickSlotSymbol
    {
        Triangle,
        Square,
        Circle
    }

    [DisallowMultipleComponent]
    public sealed class QuickSlotSymbolGraphic : MaskableGraphic
    {
        [SerializeField] private QuickSlotSymbol symbol;
        [SerializeField, Range(8, 64)] private int circleSegments = 24;

        public QuickSlotSymbol Symbol => symbol;

        public void SetSlotIndex(int slotIndex)
        {
            symbol = slotIndex switch
            {
                0 => QuickSlotSymbol.Triangle,
                1 => QuickSlotSymbol.Square,
                _ => QuickSlotSymbol.Circle
            };

            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = GetPixelAdjustedRect();
            Vector2 centre = rect.center;
            float halfSize = Mathf.Min(rect.width, rect.height) * 0.4f;

            switch (symbol)
            {
                case QuickSlotSymbol.Triangle:
                    AddTriangle(vh, centre, halfSize);
                    break;

                case QuickSlotSymbol.Square:
                    AddSquare(vh, centre, halfSize);
                    break;

                case QuickSlotSymbol.Circle:
                    AddCircle(vh, centre, halfSize);
                    break;
            }
        }

        private void AddTriangle(VertexHelper vh, Vector2 centre, float size)
        {
            AddVertex(vh, centre + new Vector2(0f, size));
            AddVertex(vh, centre + new Vector2(-size, -size));
            AddVertex(vh, centre + new Vector2(size, -size));

            vh.AddTriangle(0, 1, 2);
        }

        private void AddSquare(VertexHelper vh, Vector2 centre, float size)
        {
            AddVertex(vh, centre + new Vector2(-size, size));
            AddVertex(vh, centre + new Vector2(size, size));
            AddVertex(vh, centre + new Vector2(size, -size));
            AddVertex(vh, centre + new Vector2(-size, -size));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
        }

        private void AddCircle(VertexHelper vh, Vector2 centre, float radius)
        {
            AddVertex(vh, centre);

            int segments = Mathf.Max(8, circleSegments);

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                AddVertex(vh, centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
            }

            for (int i = 1; i <= segments; i++)
                vh.AddTriangle(0, i, i + 1);
        }

        private void AddVertex(VertexHelper vh, Vector2 position)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;
            vertex.position = position;

            vh.AddVert(vertex);
        }

        private new void OnValidate()
        {
            circleSegments = Mathf.Clamp(circleSegments, 8, 64);
            SetVerticesDirty();
        }
    }
}