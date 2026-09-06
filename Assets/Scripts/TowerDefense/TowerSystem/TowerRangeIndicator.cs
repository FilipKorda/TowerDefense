using UnityEngine;

namespace TowerDefense.BuildSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class TowerRangeIndicator : MonoBehaviour
    {
        [SerializeField, Range(8, 128)] private int segmentCount = 64;
        [SerializeField] private float yOffset = 0.05f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = segmentCount;
            gameObject.SetActive(false);
        }

        public void Show(Vector3 centerPosition, float radius)
        {
            DrawCircle(centerPosition, radius);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void DrawCircle(Vector3 center, float radius)
        {
            Vector3 origin = center + Vector3.up * yOffset;

            for (int i = 0; i < segmentCount; i++)
            {
                float angle = (i / (float)segmentCount) * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                lineRenderer.SetPosition(i, origin + new Vector3(x, 0f, z));
            }
        }
    }
}