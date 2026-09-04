using TowerDefense.PlayerSystem;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnemyRuntime))]
    public class EnemyPathFollower : MonoBehaviour
    {
        [SerializeField] private float reachDistance = 0.05f;
        [SerializeField] private bool destroyAtFinish = true;

        private EnemyPath path;
        private EnemyRuntime enemyRuntime;
        private SlowEffect slowEffect;
        private Vector3 formationOffset;
        private int targetPointIndex = 1;

        private void Awake()
        {
            enemyRuntime = GetComponent<EnemyRuntime>();
        }

        private void Update()
        {
            if (path == null || enemyRuntime == null || !enemyRuntime.IsAlive)
            {
                return;
            }

            if (targetPointIndex >= path.PointCount)
            {
                FinishPath();
                return;
            }

            Vector3 targetPoint = GetElevatedPoint(path.GetPoint(targetPointIndex) + formationOffset);
            float moveSpeed = GetCurrentMoveSpeed();

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPoint,
                moveSpeed * Time.deltaTime);

            Vector3 direction = targetPoint - transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }

            if (Vector3.Distance(transform.position, targetPoint) <= reachDistance)
            {
                targetPointIndex++;
            }
        }

        public void Initialize(EnemyPath enemyPath, Vector3 formationOffsetValue)
        {
            path = enemyPath;
            formationOffset = formationOffsetValue;
            targetPointIndex = 1;

            if (path != null)
            {
                transform.position = GetElevatedPoint(path.GetPoint(0) + formationOffset);
            }
        }

        private Vector3 GetElevatedPoint(Vector3 point)
        {
            if (enemyRuntime.Definition.MovementType == EnemyMovementType.Flying || enemyRuntime.Definition.MovementType == EnemyMovementType.Ground)
            {
                point.y += enemyRuntime.Definition.HeightAboveTheGround;
            }

            return point;
        }

        private float GetCurrentMoveSpeed()
        {
            float baseSpeed = enemyRuntime.Definition != null ? enemyRuntime.Definition.MoveSpeed : 1f;

            if (slowEffect == null)
            {
                slowEffect = GetComponent<SlowEffect>();
            }

            if (slowEffect != null)
            {
                return slowEffect.CurrentMoveSpeed;
            }

            return baseSpeed;
        }

        private void FinishPath()
        {
            if (enemyRuntime.Definition != null && PlayerStats.Instance != null)
            {
                PlayerStats.Instance.TakeDamage(enemyRuntime.Definition.DamageToPlayerBase);
            }

            if (destroyAtFinish)
            {
                Destroy(gameObject);
            }
        }
    }
}