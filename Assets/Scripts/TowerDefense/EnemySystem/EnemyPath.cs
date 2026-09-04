using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    public class EnemyPath : MonoBehaviour
    {
        [SerializeField] private Transform startPoint;
        [SerializeField] private Transform finishPoint;
        [SerializeField] private List<Transform> middlePoints = new List<Transform>();

        public Vector3 GetPoint(int index)
        {
            if (index <= 0)
            {
                return startPoint != null ? startPoint.position : transform.position;
            }

            int middleIndex = index - 1;
            if (middleIndex < middlePoints.Count)
            {
                Transform middlePoint = middlePoints[middleIndex];
                return middlePoint != null ? middlePoint.position : transform.position;
            }

            return finishPoint != null ? finishPoint.position : transform.position;
        }

        public int PointCount => 2 + middlePoints.Count;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            for (int i = 0; i < PointCount; i++)
            {
                Vector3 point = GetPoint(i);
                Gizmos.DrawSphere(point, 0.2f);

                if (i + 1 < PointCount)
                {
                    Gizmos.DrawLine(point, GetPoint(i + 1));
                }
            }
        }
    }
}
